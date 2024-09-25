using System.Collections;
using AscomPayPG.Models;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Models.Paystack;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.ViewModels;
using AscomPayPG.Services.Gateways;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using RestSharp;

namespace AscomPayPG.Services
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ILogger<PaymentProcessor> _logger;
        private readonly IHelperService _helperService;

        private IPayment? _gatewayInstance { get; set; }

        private readonly IGTPay _gtPay;
        private readonly IPaystack _paystack;
        private readonly IRepository<PaymentGateway> _paymentGatewayRepo;
        private readonly ITransactionsRepository<Transactions> _TransactionsRepo;
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;

        public PaymentProcessor(
            ILogger<PaymentProcessor> logger,
            IHelperService helperService,
            IRepository<PaymentGateway> paymentGatewayRepo,
            ITransactionsRepository<Transactions> TransactionsRepo,
            IClientRequestRepository<ClientRequest> clientRequestRepo,
            IGTPay gtPay,
            IPaystack paystack
            )
        {
            _logger = logger;
            _helperService = helperService;
            _gatewayInstance = null;
            _gtPay = gtPay;
            _paystack = paystack;
            _paymentGatewayRepo = paymentGatewayRepo;
            _TransactionsRepo = TransactionsRepo;
            _clientRequestRepo = clientRequestRepo;
        }

        private async Task<AppResult<bool>> SetGatewayInstance(PaymentGateway selectedPaymentGateway)
        {
            AppResult<bool> result = new AppResult<bool>();
            try
            {
                if (GatewayTypes.GTPay.ToString() == selectedPaymentGateway.Name)
                {
                    _gatewayInstance = _gtPay;
                }
                else if (GatewayTypes.Paystack.ToString() == selectedPaymentGateway.Name)
                {
                    _gatewayInstance = _paystack;
                }
                else
                {
                    throw new Exception($"Unabel to find and set Payment Gateway for {selectedPaymentGateway.Name}");
                }

                result.Status = _gatewayInstance == null ? false : true;
                if (!result.Status) throw new Exception($"Unable to get gateway with Name {selectedPaymentGateway.Name}");
                result.Message = "success";
            }
            catch (Exception ex)
            {
                await _helperService.CustomLogError(ex, "SetGatewayInstance");
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<AppResult<GatewayViewModel>> Pay(PaymentRequest payReq)
        {
            AppResult<GatewayViewModel> result = new AppResult<GatewayViewModel>();
            string payReqReference = string.Empty;
            try
            {

                /*
                    1. call service with serviceid to get payment amount 
                    2. call gateway or populate dropdown with payment options
                    3. create payment entry for this payment with statusDescription of Init 
                    4. prompt user to select payment gateway to use
                */
                if (payReq.Uid == null) throw new Exception($"Uid is required");
                // payReq.ProductUID = payReq.ProductUID.ToString();
                //persist request payload from client 
                ClientRequest reqPayload = new ClientRequest();
                reqPayload.CallbackURL = payReq.CallbackURL;


                var checkUser = await _clientRequestRepo.GetUser(payReq.Uid);
                if (checkUser == null) throw new Exception($"Error Invalid User");

                var checkTransaction = _helperService.GetOneTransactionType(payReq.TransactionType);
                if (checkTransaction == null) throw new Exception($"Error Invalid TransactionType");

                if (checkTransaction.TiD == 1)
                {
                    var checkDestinationAccount = _clientRequestRepo.GetUserAccount(payReq.destination);
                    if (checkDestinationAccount == null) throw new Exception($"Invalid Destination Account Number {payReq.destination}");
                }
                else if (checkTransaction.TiD == 2)
                {
                    var checkDestinationWallet = _clientRequestRepo.GetUserWallet(payReq.destination);
                    if (checkDestinationWallet == null) throw new Exception($"Invalid Destination Wallet {payReq.destination}");
                }
                else { throw new Exception($"Error Invalid TransactionType for account/wallet top up"); }


                var newReferenceId = await _helperService.GetNewReferenceId();
                if (newReferenceId == null) throw new Exception($"Error generating 'New Reference Id' for this Transactions");
                reqPayload.Reference = newReferenceId!;

                #region

                reqPayload.Uid = Guid.Parse(payReq.Uid);

                #endregion

                var reqPayloadResponse = await _clientRequestRepo.Create(reqPayload);
                if (reqPayloadResponse == null) throw new Exception($"Error creating new ClientRequest database entry");

                payReqReference = reqPayloadResponse.Reference;

                //2. call gateway or populate dropdown with payment options
                var paymentGateways = await _paymentGatewayRepo.GetAll();
                if (!paymentGateways.Any()) throw new Exception($"No Payment Gateway found!");

                //3.create payment entry for this payment with statusDescription of Init
                var description = payReq.description;

                Transactions initTransactions = _helperService.GetPaymentDefaultModel(payReq, reqPayload.Reference, description);

                Transactions dbTransactions = await _TransactionsRepo.Create(initTransactions);
                var viewModel = new GatewayViewModel(paymentGateways);
                viewModel.Description = description;
                viewModel.TransactionsAmount = dbTransactions.Amount;
                viewModel.TransactionId = dbTransactions.TransactionId;
                viewModel.CallbackURL = dbTransactions.CallbackURL;
                viewModel.Reference = dbTransactions.RequestTransactionId;
                viewModel.Email = dbTransactions.Email;

                result.Status = true;
                result.Message = "success";
                result.Data.Add(viewModel);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Pay:: {ex.Message}");
                result.Status = false;
                result.Message = ex.Message;
                var errorGatewayViewModel = new GatewayViewModel();
                errorGatewayViewModel.Reference = payReqReference;
                errorGatewayViewModel.CallbackURL = payReq.CallbackURL;
                result.Data.Add(errorGatewayViewModel);
            }
            return result;
        }

        public async Task<AppResult<PaymentResponseView>> PayInit(GatewayViewModel payInitReq)
        {
            AppResult<PaymentResponseView> result = new AppResult<PaymentResponseView>();
            PaymentResponseView payRes = new PaymentResponseView();
            try
            {
                /*
                    1. get selected payment gateway, 
                    2. get Transactions record, update and persist
                    3. get gateway processor instance 
                    4. init Transactions 
                    5. return view
                */
                var selectedPaymentGateway = await _paymentGatewayRepo.GetOne(payInitReq.GatewayId);
                if (selectedPaymentGateway == null) throw new Exception($"Unable to fetch payment gateway with id: {payInitReq.GatewayId}");

                var dbTransactions = await _TransactionsRepo.GetOne(payInitReq.TransactionId);
                if (dbTransactions == null) throw new Exception($"Unable to fetch Transactions record with id: {payInitReq.TransactionId}");

                //populate payRes 
                payRes.Reference = dbTransactions.RequestTransactionId!;
                payRes.Customer = dbTransactions.Email!;
                payRes.Gateway = selectedPaymentGateway.Name!;
                payRes.TransactionsAmount = dbTransactions.Amount;
                payRes.RequestingClientUrl = dbTransactions.CallbackURL!;



                dbTransactions.PaymentGatewayId = selectedPaymentGateway.PaymentGatewayId;
                dbTransactions.Description = $"Payment of {dbTransactions.Amount} via {selectedPaymentGateway.Name} | {dbTransactions.Description!.Split("|").Last()}";
                //dbTransactions.StatusDescription = PaymentStatus.Pending.ToString();
                dbTransactions.UpdatedAt = DateTime.Now;
                var updateStatus = await _TransactionsRepo.Update(dbTransactions, dbTransactions.TransactionId);
                if (!updateStatus) throw new Exception($"db update for Transactions with id {dbTransactions.TransactionId} failed!");
                payInitReq.CallbackURL = dbTransactions.CallbackURL;
                payInitReq.TransactionsAmount = dbTransactions.Amount;
                payInitReq.Reference = dbTransactions.RequestTransactionId;
                payInitReq.Email = dbTransactions.Email;

                //3. get gateway processor instance 
                var setGatewayStatus = await SetGatewayInstance(selectedPaymentGateway);
                if (!setGatewayStatus.Status) throw new Exception($"Unable to set Payment Gateway");
                var payResResponse = await _gatewayInstance!.Pay(payInitReq, dbTransactions, selectedPaymentGateway);
                if (!payResResponse.Status) throw new Exception(payResResponse.Message);

                result.Status = true;
                result.Message = "success";

                result.Data.Add(payResResponse);

            }
            catch (Exception ex)
            {
                await _helperService.CustomLogError(ex, "Pay");
                result.Status = false;
                result.Message = ex.Message;
                result.Data.Add(payRes);
            }
            return result;
        }

        public async Task<PayQueryResponse> QueryStatus(string queryReqReference)
        {
            PayQueryResponse result = new PayQueryResponse();
            string TransactionsCurrentStatus = string.Empty;
            try
            {
                var dbTransactions = await _TransactionsRepo.GetPayRequest(queryReqReference);
                if (dbTransactions == null) throw new Exception($"Unable to fetch Transactions requestid: {queryReqReference}");
                TransactionsCurrentStatus = dbTransactions.Status!;

                var setGatewayStatus = await SetGatewayInstance(dbTransactions.PaymentGateway);
                if (!setGatewayStatus.Status) throw new Exception($"Unable to set Payment Gateway");

                var queryStatusRes = await _gatewayInstance!.PayQuery(queryReqReference, dbTransactions.PaymentGateway);
                if (queryStatusRes == null) throw new Exception($"Unable Query Payment with reference {queryReqReference}");

                result = queryStatusRes;
            }
            catch (Exception ex)
            {
                await _helperService.CustomLogError(ex, "QueryStatus");
                result.Status = false;
                result.Message = ex.Message;
                result.TransactionId = queryReqReference;
                result.StatusDescription = TransactionsCurrentStatus;
            }
            return result;
        }

        public async Task<GatewayViewModel> GetGatewayViewData(string reference)
        {
            GatewayViewModel result = new GatewayViewModel();
            string callbackURL = string.Empty;
            try
            {
                //get payment gateways 
                var paymentGateways = await _paymentGatewayRepo.GetAll();
                result = new GatewayViewModel(paymentGateways);
                ClientRequest? clientPayload = await _clientRequestRepo.GetClientReference(reference);
                callbackURL = clientPayload.CallbackURL;
                var Transactions = await _TransactionsRepo.GetPayRequest(reference);
                if (Transactions == null) throw new Exception($"Transactions with ref {reference} does not exist");

                result.Email = Transactions.Email;
                result.Reference = reference;
                result.TransactionsAmount = Transactions.Amount;
                //result.Description = Transactions.Description;
                result.CallbackURL = Transactions.CallbackURL;
                result.TransactionId = Transactions.TransactionId;

            }
            catch (Exception ex)
            {
                await _helperService.CustomLogError(ex, "GetGatewayViewData");
                result.CallbackURL = callbackURL;
            }
            return result;
        }

        public async Task<PayStatusViewModel> ProcessCallBack(string TransactionsReference)
        {
            PayStatusViewModel result = new PayStatusViewModel();
            try
        {
                /*
                    1. get entry from db
                    2. using payment gateway instance, call gateway PayQuery 
                    3. update db for success 
                    4. call db record callback url 
                */

                var Transactions = await _TransactionsRepo.GetPayRequest(TransactionsReference);
                result.Reference = TransactionsReference;
                if (Transactions == null) throw new Exception($"Transactions with ref {TransactionsReference} does not exist");
                //result.CallbackURL = Transactions.CallbackURL + $"?reference={Transactions.RequestTransactionId}";
                result.CallbackURL = !string.IsNullOrEmpty(Transactions.UserUID.ToString()) ? Transactions.CallbackURL : Path.Join(Transactions.CallbackURL, $"{Transactions.RequestTransactionId}");
                var queryResponse = await QueryStatus(Transactions.RequestTransactionId!);

                    if (queryResponse.Status)
                    {
                        //close request
                        var checkClient = await _clientRequestRepo.GetClientReference(TransactionsReference);
                        if (checkClient == null) throw new Exception($"Transactions with ref {TransactionsReference} was not logged");
                        if (checkClient.Status != true)
                        {
                            //update user wallet/account
                            var updateUserAccount = await _TransactionsRepo.UpdateUserAccount(Transactions.TransactionId);
                            if (updateUserAccount.Item1 == false) throw new Exception(updateUserAccount.Item3);
                        }
                    }

                    result.Status = queryResponse.Status;
                    result.StatusDescription = queryResponse.StatusDescription;

                    var logMessage = $"CallbackURL : {result.CallbackURL}, {queryResponse.TransactionId} update was " + ((queryResponse.Status) ? "Successful" : "Failed");
                    _logger.LogInformation(logMessage);

            }
            catch (Exception ex)
            {
                result.Status = false;
                var errormessage = $"ProcessCallBack:: with TransactionsReference: {TransactionsReference} failed. {ex.Message}";
                await _helperService.CustomLogError(ex, $"ProcessCallBack. {errormessage}");
            }
            return result;
        }
    }
}