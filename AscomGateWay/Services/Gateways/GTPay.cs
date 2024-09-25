using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.DTOs;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.ViewModels;
using AscomPayPG.Services.Gateways;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace AscomPayPG.Services
{
    public class GTPay : IGTPay
    {
        private readonly ILogger<GTPay> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITransactionsRepository<Transactions> _TransactionsRepo;
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;
        private readonly IHelperService _helperService;
        private readonly IEncodeValue _encode;
        private readonly IRepository<PaymentGateway> _paymentGatewayRepo;

        private long _paymentGatewayId { get; set; }

        public GTPay(
            ILogger<GTPay> logger,
            IConfiguration configuration,
            ITransactionsRepository<Transactions> TransactionsRepo,
            IRepository<PaymentGateway> paymentGatewayRepo,
            IHelperService helperService,
            IClientRequestRepository<ClientRequest> clientRequestRepo,
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

                //string url = _configuration["GatewayOptions:GTPay:PostingURL"];
                string url = selectedGateway.PayUrl!;
                string gtpublicKey = _configuration["GatewayOptions:GTPay:PublicKey"];
                Guid Token = Guid.Parse(_configuration["App:decrpt:TokenData"]);

                ResponseMessage publicKey = await _encode.decrypt(gtpublicKey,3, Token);

                if (publicKey.isOk == false) throw new Exception($"Payment Initialization failed. {publicKey.Message}");

                GTPayRequest requestData = new GTPayRequest();
                requestData.amount = Convert.ToDouble(payReq.TransactionsAmount * 100);
                requestData.email = payReq.Email!;
                requestData.currency = "NGN";
                requestData.callback_url = _configuration["App:CallbackURL"];
                requestData.transaction_ref = payReq.Reference!;

                Dictionary<string, string> headerOptions = new Dictionary<string, string>()
                {
                    { "Authorization", publicKey.Message},
                    { "Content-Type", "application/json"}
                };

                var postResponse = await _helperService.PostResource(url, headerOptions, requestData);
                if (!postResponse.Status) throw new Exception($"Post Payment Initialization failed! {postResponse.Message}");
                //_logger.LogInformation(JsonConvert.SerializeObject(postResponse.Data.First()));
                GTPayResponse? responseModel = JsonConvert.DeserializeObject<GTPayResponse>(postResponse.Data.First());
                if (responseModel?.status != 200) throw new Exception($"Payment Initialization failed. {responseModel?.message}");

                dbTransactions.Status = PaymentStatus.Pending.ToString();
                dbTransactions.StatusId = 2;
                dbTransactions.UpdatedAt = DateTime.Now;

                string lastPart = string.Empty;

                int lastSlashIndex = responseModel.data.checkout_url.LastIndexOf('/');
                if (lastSlashIndex != -1 && lastSlashIndex < responseModel.data.checkout_url.Length - 1)
                {
                    lastPart = responseModel.data.checkout_url.Substring(lastSlashIndex + 1);
                }

                if (lastPart != payReq.Reference)
                {
                    dbTransactions.RequestTransactionId = lastPart;

                    ClientRequest? clientPayload = await _clientRequestRepo.GetClientReference(payReq.Reference);

                    if (clientPayload == null) throw new Exception($"Transactions with ref {payReq.Reference} does not exist");

                    clientPayload.Reference = lastPart;

                    bool updateClientStatus = await _clientRequestRepo.Update(clientPayload, clientPayload.ClientRequestId);

                    if (!updateClientStatus) throw new Exception($"ClientStatus update for {dbTransactions.RequestTransactionId} failed!");
                }


                bool updateStatus = await _TransactionsRepo.Update(dbTransactions, dbTransactions.TransactionId);


                if (!updateStatus) throw new Exception($"Transactions update for {dbTransactions.RequestTransactionId} failed!");

                result.TransactionsAmount = dbTransactions.Amount;
                result.Gateway = dbTransactions.PaymentGateway.Name!;
                result.Reference = dbTransactions.RequestTransactionId!;
                result.RequestingClientUrl = string.IsNullOrEmpty(dbTransactions.CallbackURL) ? _configuration["App:HomePage"] : dbTransactions.CallbackURL; 
                result.Customer = dbTransactions.Email!;

                result.Status = true;
                result.Message = PaymentStatus.Pending.ToString();
                result.Data = responseModel.data.checkout_url;
            }
            catch (Exception ex)
            {
                await _helperService.CustomLogError(ex, "GTPay.Pay");
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

                var doNotMakeExternalApiCallText = new List<string>() { "failed", "declined", "approved" };
                //if (payRequest.StatusDescription == PaymentStatus.Pending.ToString())
                if (!doNotMakeExternalApiCallText.Contains(payRequest.Status!.ToLower()))
                {
                    //string url = _configuration["GatewayOptions:GTPay:ReQueryUrl"];
                    string url = selectedGateway.QueryUrl!;
                    string gtprivateKey = _configuration["GatewayOptions:GTPay:PrivateKey"];
                    Guid Token = Guid.Parse(_configuration["App:decrpt:TokenData"]);
                    ResponseMessage privateKey = await _encode.decrypt(gtprivateKey, 3, Token);

                    if (privateKey.isOk == false) throw new Exception($"Payment Initialization failed. {privateKey.Message}");


                    //queryReq.TransactionId is the referenceCode
                    url = url + referenceCode;

                    Dictionary<string, string> getOptions = new Dictionary<string, string>()
                    {
                        {"Authorization", privateKey.Message},
                        {"Content-Type", "application/json"}
                    };

                    var response = await _helperService.GetResource(url, getOptions);

                    if (!response.Status) throw new Exception($"{GatewayTypes.GTPay.ToString()} Payment verification failed for id: {queryReqReference}");

                    GTPayQueryResponse? responseModel = JsonConvert.DeserializeObject<GTPayQueryResponse>(response.Data.First());
                    if (responseModel == null || responseModel?.status != 200) throw new Exception($"Payment verification failed. {responseModel?.message}");
                    if (responseModel.data.transaction_status.ToLower() == GTPayTransactionstatusEnum.Success.ToString().ToLower())
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
                        var transResponse = responseModel.data.transaction_status.ToLower();
                        var failedText = new List<string>() { "failed", "declined" };
                        payRequest.Status = failedText.Contains(transResponse) ? PaymentStatus.Declined.ToString() : payRequest.Status;
                        payRequest.StatusId = failedText.Contains(transResponse) ? 3 : payRequest.StatusId;
                        payRequest.TransactionStatus = false;
                        payRequest.UpdatedAt = DateTime.Now;
                        bool updateSucceeded = await _TransactionsRepo.Update(payRequest, payRequest.TransactionId);
                        result.StatusDescription = payRequest.Status!;
                        result.Status = false;
                        _logger.LogInformation($"{transResponse} | Update for {referenceCode} pay request was successful");

                    }

                    //result.Message = JsonConvert.SerializeObject(responseModel?.data);
                    result.Message = payRequest.Status!;
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
                await _helperService.CustomLogError(ex, "GTPay.PayQuery");
                result.Status = false;
                result.Message = ex.Message;
                result.TransactionId = queryReqReference;
                result.StatusDescription = TransactionsCurrentStatus;
            }
            return result;
        }

        public static void Webhook(VirtualAccount_VM chargeResponse)
        {
            chargeResponse = new VirtualAccount_VM()
            {
                transaction_reference = "REFE52ARZHTS/1668421222619_1",
                virtual_account_number = "2129125316",
                principal_amount = "222.00",
                settled_amount = "221.78",
                fee_charged = "0.22",
                transaction_date = "2022-11-14T10:20:22.619Z",
                customer_identifier = "SBN1EBZEQ8",
                transaction_indicator = "C",
                remarks = "Transfer FROM sandbox sandbox | [SBN1EBZEQ8] TO sandbox sandbox",
                currency = "NGN",
                channel = "virtual-account",
                meta = new MetaBody_VM()
                {
                    freeze_transaction_ref = null,
                    reason_for_frozen_transaction = null
                },
                encrypted_body = "ViASuHLhO+SP3KtmcdAOis+3Obg54d5SgCFPFMcguYfkkYs/i44jeT5Dbx52TcOvHRp9HlnCoFwbATkEihzv2C8UyPoC38sRb90S5Z9Fq7vRwjDQz/hYi/nKbWA0btPr3A+UXhX1Nu5ek+TL0ENUC8W1ZX/FrowX3HQaYiwe3tU/Kfr2XvAGwT7IAx5CQBhpzL34faHP4jbwSVmSgVYmW5rd2ClWQ7WWJjDMakrqYJva8qd0vhkqSpyz2KywOV9t9zSHRx3VpbvlDsBdkNGr+4Axh/7Gspu3xo9mMOIdv73OzjN4VA/qQP+fQMCjU1pbS8oh81HjwkHjzC5SBhzR8IU8bsmvFUyzJMfDoJuUB+fs09SLW7pdfODwK5vB8LtdKPnAuTPlv5dHVAPeMG/ubtl/HOqCZs4axjuO557srw0GpKk86bwaVKt4IQ17nY/QCJFC273HWU1CawP7d3nQasRZf/TU7ra+fOjQBHQ7Gtz2Pnfp3gLljBKenMT4Cabks1X2/6ZQpd/yGFkloYdS7ZW3kEvrorjcyma4WNDmJfhcdR9XGsom6Y/M/n/gMMa0z2KPbHDRoEBeRYbQHcnu5LnGWzBA4Y4RMSTDesD876PDB1bOnMzNPrWYam6ZVRHz"
            };

            String SerializedPayload = JsonConvert.SerializeObject(chargeResponse);
            Console.WriteLine(SerializedPayload);
            string result = "";
            var secretKeyBytes = Encoding.UTF8.GetBytes("sandbox_sk_9ac9418e847972dd45f5fe845b5716ef305589808eda");
            var inputBytes = Encoding.UTF8.GetBytes(SerializedPayload);
            var hmac = new HMACSHA512(secretKeyBytes);
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            result = BitConverter.ToString(hashValue).Replace("-", string.Empty);
            Console.WriteLine(result);

            Console.WriteLine(result.ToLower() == "18b9eb6ca68f92ca9f058da7bce6545efb12660cf75f960e552cf6098bb5ee8e71f20331dcfe0dfaea07439cc6629f901850291a39f374a1bd076c4eff1026c8");
        }

    }
}