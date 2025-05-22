using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Helpers;
using AscomPayPG.Models;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.VasModels;
using AscomPayPG.Services.Gateways.Interface;
using AscomPayPG.Services.Interface;
using PaymentGateWayMiddleWare.Model;

namespace AscomPayPG.Services.Implementation
{
    public class VasService : IVasService
    {
        private readonly I9psbVaS _vasService;
        private readonly AppDbContext _appDbContext;
        private readonly ITransactionHelper  _transactionHelper;
        private readonly IEmailNotification _emailNotification;
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;

        public VasService(I9psbVaS vasService, AppDbContext appDbContext, 
                         ITransactionHelper transactionHelper,
                         IClientRequestRepository<ClientRequest> clientRequestRepo,
                         IEmailNotification emailNotification)
        {
            _vasService = vasService;
            _appDbContext = appDbContext;
            _transactionHelper = transactionHelper;
            _emailNotification = emailNotification;
            _clientRequestRepo = clientRequestRepo;
        }

        public async Task<Nine9psbListGenResponse<DataPlansResponse>> GetDataPlans(string phoneNumber) => await  _vasService.GetDataPlans(phoneNumber);
        public async Task<Nine9psbGenResponse<PhoneNumberLookUpResponse>> GetPhoneNumberNetwork(string phoneNumber) => await _vasService.GetPhoneNetwork(phoneNumber);
        public Task<Nine9psbGenResponse<StatusResponse>> GetTopUpStatus(string transReference) => _vasService.GetTopUpStatus(transReference);
        public async Task<Nine9psbGenResponse<AirtimeTopUp>> PurchaseAirtime(AirTimeTopUpRequest requestModel, string userId)
        {
            var paymentProviderCharges = await _transactionHelper.GetPaymentProviderCharges(TransactionTypes.AirTimeTopUp.ToString());
            var marchantCharge = await _transactionHelper.CalculateCharges(decimal.Parse(requestModel.amount), TransactionTypes.AirTimeTopUp.ToString());
            var charges = paymentProviderCharges + marchantCharge;
            var vat = await _transactionHelper.CalculateVAT(decimal.Parse(requestModel.amount) + charges, TransactionTypes.AirTimeTopUp.ToString());

            try
            {
                var account = _appDbContext.Accounts.FirstOrDefault(acc => acc.UserUid.ToString() == userId);
                if (account == null)
                {
                    return new Nine9psbGenResponse<AirtimeTopUp>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User's account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                var user = _appDbContext.Users.FirstOrDefault(x => x.UserUid == account.UserUid);
                if (user == null)
                {
                    return new Nine9psbGenResponse<AirtimeTopUp>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User  does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                if (account.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new Nine9psbGenResponse<AirtimeTopUp>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Insufficient balance",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                var response =  await _vasService.PurchaseAirtime(requestModel);
                if (response.IsSuccessful)
                {

                    var debit = await _clientRequestRepo.BuildDebit(decimal.Parse(requestModel.amount), paymentProviderCharges, marchantCharge, "Airtime Purchae/"+requestModel.phoneNumber,
                                                                      requestModel.transactionReference, decimal.Parse(requestModel.amount) + vat + charges, TransactionTypes.AirTimeTopUp.ToString(), TransactionTypes.AirTimeTopUp.ToString(),
                                                                      account.AccountName,
                                                                      vat, charges, PaymentProvider.NinePSB.ToString(),
                                                                      account.UserUid.ToString(),
                                                                      account.AccountNumber);

                    var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { debit });

                    var balance = UpdateBalance(decimal.Parse(requestModel.amount), account);
                    await _transactionHelper.NotifyForDebitSMS(user, account.AccountNumber, requestModel.amount, balance.ToString(), $"Airtime Purchase");
                }

                return response;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<AirtimeTopUp>
                {
                    Data = null,
                    IsSuccessful = false,
                    Message = ex.Message,
                    ResponseCode = StatusCodes.Status400BadRequest
                };
            }
        }
        public async Task<Nine9psbGenResponse<DataTopUpResponse>> PurchaseData(DataTopUpRequest requestModel, string userId)
        {

            var paymentProviderCharges = await _transactionHelper.GetPaymentProviderCharges(TransactionTypes.DataTopUp.ToString());
            var marchantCharge = await _transactionHelper.CalculateCharges(decimal.Parse(requestModel.amount), TransactionTypes.DataTopUp.ToString());
            var charges = paymentProviderCharges + marchantCharge;
            var vat = await _transactionHelper.CalculateVAT(decimal.Parse(requestModel.amount) + charges, TransactionTypes.DataTopUp.ToString());

            try
            {
                var account = _appDbContext.Accounts.FirstOrDefault(acc => acc.UserUid.ToString() == userId);
                if (account == null)
                {
                    return new Nine9psbGenResponse<DataTopUpResponse>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User's account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                var user = _appDbContext.Users.FirstOrDefault(x => x.UserUid == account.UserUid);
                if (user == null)
                {
                    return new Nine9psbGenResponse<DataTopUpResponse>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                if (account.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new Nine9psbGenResponse<DataTopUpResponse>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Insufficient balance",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                var response =  await _vasService.PurchaseDataPlan(requestModel);

                if (response.IsSuccessful)
                {
                    var debit = await _clientRequestRepo.BuildDebit(decimal.Parse(requestModel.amount), paymentProviderCharges, marchantCharge, "Data Purcharse/"+requestModel.phoneNumber,
                                                                      requestModel.transactionReference, decimal.Parse(requestModel.amount) + vat + charges, TransactionTypes.DataTopUp.ToString(), TransactionTypes.DataTopUp.ToString(),
                                                                      account.AccountName,
                                                                      vat, charges, PaymentProvider.NinePSB.ToString(),
                                                                      account.UserUid.ToString(),
                                                                      account.AccountNumber);

                    var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { debit });
                    var balance = UpdateBalance(decimal.Parse(requestModel.amount), account);
                    await _transactionHelper.NotifyForDebitSMS(user, account.AccountNumber, requestModel.amount, balance.ToString(), $"Data Purchase");
                }

                return response;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<DataTopUpResponse>
                {
                    Data = null,
                    IsSuccessful = false,
                    Message = ex.Message,
                    ResponseCode = StatusCodes.Status400BadRequest
                };
            }
        }

        #region BILLER
        public async Task<Nine9psbGenResponse<BillerPaymentResponse>> InitBillerPayment(InitaiteBillPaymentRequest requestModel)
        {
            try
            {

                var paymentProviderCharges = await _transactionHelper.GetPaymentProviderCharges(TransactionTypes.Biller.ToString());
                var marchantCharge = await _transactionHelper.CalculateCharges(decimal.Parse(requestModel.amount), TransactionTypes.Biller.ToString());
                var charges = paymentProviderCharges + marchantCharge;
                var vat = await _transactionHelper.CalculateVAT(decimal.Parse(requestModel.amount) + charges, TransactionTypes.Biller.ToString());

                var debitAccount = _appDbContext.Accounts.FirstOrDefault(acc => acc.AccountNumber == requestModel.debitAccount);
                if (debitAccount == null)
                {
                    return new Nine9psbGenResponse<BillerPaymentResponse>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Debit account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                var user = _appDbContext.Users.FirstOrDefault(x => x.UserUid == debitAccount.UserUid);
                if (user == null)
                {
                    return new Nine9psbGenResponse<BillerPaymentResponse>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Debit account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                if (debitAccount?.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new Nine9psbGenResponse<BillerPaymentResponse>
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Debit account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                requestModel.customerPhone = user.PhoneNumber;
                requestModel.debitAccount = debitAccount.AccountNumber;

                var response =  await _vasService.InitBillerPayment(requestModel);

                if (response.IsSuccessful)
                {
                    var debit = await _clientRequestRepo.BuildDebit(decimal.Parse(requestModel.amount), paymentProviderCharges, marchantCharge, "biller/"+requestModel.billerId,
                                                  requestModel.transactionReference, decimal.Parse(requestModel.amount) + vat + charges, TransactionTypes.DataTopUp.ToString(), TransactionTypes.Biller.ToString(),
                                                  debitAccount.AccountName,
                                                  vat, charges, PaymentProvider.NinePSB.ToString(),
                                                  debitAccount.UserUid.ToString(),
                                                  debitAccount.AccountNumber);

                    var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { debit });
                    var balance =  UpdateBalance(decimal.Parse(requestModel.amount), debitAccount);

                   var template =  EmailComposer.ComposeEmailForMeterToken(user.LastName, response.Data.Token);
                    if (user.IsNotificationEnabled.Value)
                    {
                       await _emailNotification.NewSendEmail("AscomPay", "Bill Payment", template, user.Email );
                       await _transactionHelper.NotifyForDebitSMS(user, debitAccount.AccountNumber, requestModel.amount, balance.ToString(), $"Bills Payment/{requestModel.billerId}");

                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<BillerPaymentResponse>
                {
                    Data = null,
                    IsSuccessful = false,
                    Message = "Debit account does not exist",
                    ResponseCode = StatusCodes.Status400BadRequest
                };
            }
        }
        private decimal UpdateBalance(decimal debitAmount, Models.DTO.Account debitAccount)
        {
            var previousBalance = debitAccount.CurrentBalance;
            debitAccount.LegerBalance = debitAccount.LegerBalance - debitAmount;
            debitAccount.CurrentBalance = debitAccount.CurrentBalance - debitAmount;
            debitAccount.PrevioseBalance = previousBalance;
            _appDbContext.Accounts.Update(debitAccount);
            _appDbContext.SaveChanges();

            return (decimal)debitAccount.LegerBalance;
        }

        public async Task<Nine9psbGenResponse<BillerFieldValidationResponse>> VaildateBillerInputFields(ValidateBillerInputRequest requestModel)
        {
            try
            {

                return await _vasService.VaildateBillerInputFields(requestModel);
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<BillerFieldValidationResponse>
                {
                    Data = null,
                    IsSuccessful = false,
                    Message = ex.Message,
                    ResponseCode = StatusCodes.Status400BadRequest
                };
            }
        }
        public async Task<Nine9psbListGenResponse<Category>> GetCategory() => await _vasService.GetCategory();  
        public async Task<Nine9psbListGenResponse<CategoryBiller>> GetCategoryBiller(string categoryId) => await _vasService.GetCategoryBiller(categoryId);
        public async Task<Nine9psbListGenResponse<BillerField>> GetBillerInputFields(string billerId) => await _vasService.GetBillerInputFields(billerId);
        public async Task<Nine9psbGenResponse<TransactionStatusResponse>> GetBillerPaymentStatus(string transactionReferenceId) => await _vasService.GetBillerPaymentStatus(transactionReferenceId);
        #endregion
    }
}
