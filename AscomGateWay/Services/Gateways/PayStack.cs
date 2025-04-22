using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Paystack;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;

namespace AscomPayPG.Services.Gateways
{
    public class Paystack : IPaystack
    {
        private readonly ILogger<Paystack> _logger;
        private readonly IConfiguration _configuration;
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;
        private readonly ITransactionsRepository<Transactions> _TransactionsRepo;
        private readonly IHelperService _helperService;
        private readonly IEncodeValue _encode;
        private readonly IRepository<PaymentGateway> _paymentGatewayRepo;

        private long _paymentGatewayId { get; set; }

        public Paystack(
            ILogger<Paystack> logger,
            IConfiguration configuration,
            IClientRequestRepository<ClientRequest> clientRequestRepo,
            ITransactionsRepository<Transactions> TransactionsRepo,
            IRepository<PaymentGateway> paymentGatewayRepo,
            IHelperService helperService,
            IEncodeValue encode
        )
        {
            _encode = encode;
            _logger = logger;
            _configuration = configuration;
            _TransactionsRepo = TransactionsRepo;
            _helperService = helperService;
            _paymentGatewayRepo = paymentGatewayRepo;
            _clientRequestRepo = clientRequestRepo;
        }
        public async Task<PaymentResponseView> Pay(GatewayViewModel payReq, Transactions dbTransactions, PaymentGateway selectedGateway)
        {
            PaymentResponseView result = new PaymentResponseView();
            try
            {
                /*
                    1. initialize Transactions
                    2. update Transactions.statusDescription to Pending
                    3. return amongs other properties, response.Data.authorization_url
                */

                //string url = _configuration["GatewayOptions:Paystack:PostingURL"];
                string url = selectedGateway.PayUrl!;
                string payKey = _configuration["GatewayOptions:Paystack:Key"];
                Guid Token = Guid.Parse(_configuration["App:decrpt:TokenData"]);

                ResponseMessage paystackKey = await _encode.decrypt(payKey, 3, Token);

                if (paystackKey.isOk == false) throw new Exception($"Payment Initialization failed. {paystackKey.Message}");


                PaystackPayRequest requestData = new PaystackPayRequest();
                requestData.amount = Convert.ToString(payReq.TransactionsAmount * 100);
                requestData.email = payReq.Email;
                //requestData.callback = payReq.CallbackURL;
                requestData.callback = _configuration["App:CallbackURL"];
                requestData.reference = payReq.Reference;

                Dictionary<string, string> headerOptions = new Dictionary<string, string>()
                {
                    { "Authorization", paystackKey.Message},
                    { "Content-Type", "application/json"}
                };

                var postResponse = await _helperService.PostResource(url, headerOptions, requestData);
                if (!postResponse.Status) throw new Exception($"Post Payment Initialization failed! {postResponse.Message}");
                _logger.LogInformation(JsonConvert.SerializeObject(postResponse.Data.First()));
                PaystackPayResponse? responseModel = JsonConvert.DeserializeObject<PaystackPayResponse>(postResponse.Data.First());
                if (responseModel?.status != true) throw new Exception($"Payment Initialization failed. {responseModel?.message}");

                dbTransactions.Status = PaymentStatus.Pending.ToString();
                dbTransactions.StatusId = 2;
                dbTransactions.UpdatedAt = DateTime.Now;
                bool updateStatus = await _TransactionsRepo.Update(dbTransactions, dbTransactions.TransactionId);
                if (!updateStatus) throw new Exception($"Transactions update for {dbTransactions.RequestTransactionId} failed!");


                result.TransactionsAmount = dbTransactions.Amount;
/*                result.Gateway = dbTransactions.PaymentGateway.Name;
*/                result.Reference = dbTransactions.RequestTransactionId;
                result.RequestingClientUrl = string.IsNullOrEmpty(dbTransactions.CallbackURL) ? _configuration["App:HomePage"] : dbTransactions.CallbackURL;
                result.Customer = dbTransactions.Email!;

                result.Status = responseModel.status;
                result.Message = PaymentStatus.Pending.ToString();
                result.Data = responseModel.data.authorization_url;

            }
            catch (Exception ex)
            {
                await _helperService.CustomLogError(ex, "Paystack.Pay");
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<PayQueryResponse> PayQuery(string queryReqReference, PaymentGateway selectedGateway)
        {
            PayQueryResponse result = new PayQueryResponse();
            string TransactionsCurrentStatus = string.Empty;
            try
            {
                /*
                    1. check Transactions for this transanction 
                        based on StatusDescription 
                        a. if Init, return Transactions Not Posted 
                        b. if Pending, make api call to external gateway 
                            - update db if status is success or declined 
                            - return status 
                        c. if Approved or Declined, return response based on db status 
                */
                //dynamic resp = new ExpandoObject();
                string? referenceCode = queryReqReference;
                if (string.IsNullOrEmpty(referenceCode)) throw new Exception($"No TransactionId/Reference code passed");

                var payRequest = await _TransactionsRepo.GetPayRequest(referenceCode);
                if (payRequest == null) throw new Exception($"Pay record with RequestTransactions Id: {referenceCode} does not exist!");
                if (payRequest.Status == PaymentStatus.Init.ToString()) throw new Exception($"Transactions RequestTransactions Id: {referenceCode} was not posted!");
                TransactionsCurrentStatus = payRequest.Status!;
                if (payRequest.Status == PaymentStatus.Pending.ToString())
                {
                    //string url = _configuration["GatewayOptions:Paystack:ReQueryUrl"];
                    string url = selectedGateway.QueryUrl!;
                    string payKey = _configuration["GatewayOptions:Paystack:Key"];
                    Guid Token = Guid.Parse(_configuration["App:decrpt:TokenData"]);

                    ResponseMessage paystackKey = await _encode.decrypt(payKey, 3, Token);

                    if (paystackKey.isOk == false) throw new Exception($"Payment Initialization failed. {paystackKey.Message}");

                    //queryReq.TransactionId is the referenceCode
                    url = url + referenceCode;

                    Dictionary<string, string> getOptions = new Dictionary<string, string>()
                    {
                        {"Authorization", paystackKey.Message},
                        {"Content-Type", "application/json"}
                    };
                    var response = await _helperService.GetResource(url, getOptions);

                    // var client = new RestClient(url);
                    // var request = new RestRequest(string.Empty, Method.Get);
                    // request.AddHeader("Authorization", paystackKey);
                    // request.AddHeader("cache-control", "no-cache");
                    // request.AddHeader("Content-Type", "application/json");
                    // RestResponse response = client.Execute(request);
                    if (!response.Status) throw new Exception($"{GatewayTypes.Paystack.ToString()} Payment verification failed for id: {queryReqReference}");

                    //PaystackPayQueryResponse? responseModel = new PaystackPayQueryResponse();
                    PaystackPayQueryResponse? responseModel = JsonConvert.DeserializeObject<PaystackPayQueryResponse>(response.Data.First());
                    if (responseModel == null || responseModel?.status != true) throw new Exception($"Payment verification failed. {responseModel?.message}");
                    if (responseModel.status == true && responseModel.data.status.ToLower() == "success")
                    {
                        //handle success 
                        payRequest.Status = PaymentStatus.Approved.ToString();
                        payRequest.StatusId = 4;
                        payRequest.TransactionStatus = true;
                        payRequest.UpdatedAt = DateTime.Now;

                        bool updateSucceeded = await _TransactionsRepo.Update(payRequest, payRequest.TransactionId);

                        if (updateSucceeded == true)
                        {
                            //update user wallet/account
                            var updateUserAccount = await _TransactionsRepo.UpdateUserAccount(payRequest.TransactionId);

                            //close request
                            var checkClient = await _clientRequestRepo.GetClientReference(payRequest.RequestTransactionId);
                            //close request by updateing the status

                            checkClient.Status = true;
                            checkClient.StatusDescription = PaymentStatus.Approved.ToString();
                            var UpdateClient = await _clientRequestRepo.Update(checkClient, checkClient.ClientRequestId);

                            _logger.LogInformation($"Found Transactions with ID {payRequest.TransactionId} for query update");

                            result.Status = true;
                            result.StatusDescription = payRequest.Status;
                            _logger.LogInformation($"Success Update for {referenceCode} pay request was successful");
                        }
                        else
                        {
                            result.Status = false;
                            result.StatusDescription = payRequest.Status;

                            _logger.LogInformation($"Either Failed to find Transactions with ID {payRequest.TransactionId} Or DataBase Update Query failed");
                        }
                    }
                    else
                    {

                        var failedText = new List<string>() { "failed", "declined" };
                        var transResponse = responseModel.data.status.ToLower();
                        payRequest.Status = failedText.Contains(transResponse) ? PaymentStatus.Declined.ToString() : payRequest.Status;
                        payRequest.TransactionStatus = false;
                        payRequest.UpdatedAt = DateTime.Now;
                        bool updateSucceeded = await _TransactionsRepo.Update(payRequest, payRequest.TransactionId);
                        result.StatusDescription = payRequest.Status;
                        result.Status = false;
                        _logger.LogInformation($"{transResponse} | Update for {referenceCode} pay request was successful");

                    }

                    //result.Message = JsonConvert.SerializeObject(responseModel?.data);
                    result.Message = payRequest.Status;
                    result.TransactionId = payRequest.RequestTransactionId!;
                    _logger.LogInformation($"PayQuery response: {result.Message}");
                }
                else
                {
                    result.Status = (bool)payRequest.TransactionStatus;
                    result.Message = $"{payRequest.RequestTransactionId}: {payRequest.Description} was {payRequest.Status}";
                    result.TransactionId = payRequest.RequestTransactionId!;
                    result.StatusDescription = payRequest.Status!;

                }

            }
            catch (Exception ex)
            {
                await _helperService.CustomLogError(ex, "Paystack.PayQuery");
                result.Status = false;
                result.Message = ex.Message;
                result.TransactionId = queryReqReference;
                result.StatusDescription = TransactionsCurrentStatus;
            }
            return result;
        }


    }
}