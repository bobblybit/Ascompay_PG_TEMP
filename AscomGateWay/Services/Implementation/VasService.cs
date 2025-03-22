using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Helpers;
using AscomPayPG.Models;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.VasModels;
using AscomPayPG.Services.Gateways.Interface;
using AscomPayPG.Services.Interface;

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

        public Task<PlainResponse> GetDataPlans(string phoneNumber) => _vasService.GetDataPlans(phoneNumber);
        public Task<PlainResponse> GetPhoneNumberNetwork(string phoneNumber) => _vasService.GetPhoneNetwork(phoneNumber);
        public Task<PlainResponse> GetTopUpStatus(string transReference) => _vasService.GetTopUpStatus(transReference);
        public async Task<PlainResponse> PurchaseAirtime(AirTimeTopUpRequest requestModel, string userId)
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
                    return new PlainResponse
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
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User  does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                if (account.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new PlainResponse
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
                return new PlainResponse
                {
                    Data = null,
                    IsSuccessful = false,
                    Message = ex.Message,
                    ResponseCode = StatusCodes.Status400BadRequest
                };
            }
        }
        public async Task<PlainResponse> PurchaseData(DataTopUpRequest requestModel, string userId)
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
                    return new PlainResponse
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
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                if (account.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new PlainResponse
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
                return new PlainResponse
                {
                    Data = null,
                    IsSuccessful = false,
                    Message = ex.Message,
                    ResponseCode = StatusCodes.Status400BadRequest
                };
            }
        }

        #region BILLER
        public async Task<PlainResponse> InitBillerPayment(InitaiteBillPaymentRequest requestModel)
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
                    return new PlainResponse
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
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                //  var account = _appDbContext.Accounts.FirstOrDefault(acc => acc.UserUid.ToString() == requestModel.customerId);
                if (debitAccount?.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Insufficient balance",
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

                   var template =  EmailComposer.ComposeEmailForMeterToken(user.LastName, response.Data.token);
                   await _emailNotification.NewSendEmail("AscomPay", "Bill Payment", template, user.Email );
                   await _transactionHelper.NotifyForDebitSMS(user, debitAccount.AccountNumber, requestModel.amount, balance.ToString(), $"Bills Payment/{requestModel.billerId}");
                }

                return response;
            }
            catch (Exception ex)
            {
                return new PlainResponse
                {
                    Data = null,
                    IsSuccessful = false,
                    Message = ex.Message,
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

        public async Task<PlainResponse> VaildateBillerInputFields(ValidateBillerInputRequest requestModel)
        {
            try
            {
               /* var user = _appDbContext.Users.FirstOrDefault(acc => acc.UserUid.ToString() == requestModel.customerId);
                if (user == null)
                {
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User's does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                var account = _appDbContext.Accounts.FirstOrDefault(acc => acc.UserUid.ToString() == requestModel.customerId);*/

               /* if (account?.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Insufficient balance",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }*/

                return await _vasService.VaildateBillerInputFields(requestModel);
            }
            catch (Exception ex)
            {
                return new PlainResponse
                {
                    Data = null,
                    IsSuccessful = false,
                    Message = ex.Message,
                    ResponseCode = StatusCodes.Status400BadRequest
                };
            }
        }
        public async Task<PlainResponse> GetCategory() => await _vasService.GetCategory();  
        public async Task<PlainResponse> GetCategoryBiller(string categoryId) => await _vasService.GetCategoryBiller(categoryId);
        public async Task<PlainResponse> GetBillerInputFields(string billerId) => await _vasService.GetBillerInputFields(billerId);
        public async Task<PlainResponse> GetBillerPaymentStatus(string transactionReferenceId) => await _vasService.GetBillerPaymentStatus(transactionReferenceId);
        #endregion
    }
}
