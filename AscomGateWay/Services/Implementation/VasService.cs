using AscomPayPG.Data;
using AscomPayPG.Models.DTO;
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

        public VasService(I9psbVaS vasService, AppDbContext appDbContext)
        {
            _vasService = vasService;
            _appDbContext = appDbContext;
        }

        public Task<PlainResponse> GetDataPlans(string phoneNumber) => _vasService.GetDataPlans(phoneNumber);
        public Task<PlainResponse> GetPhoneNumberNetwork(string phoneNumber) => _vasService.GetPhoneNetwork(phoneNumber);
        public Task<PlainResponse> GetTopUpStatus(string transReference) => _vasService.GetTopUpStatus(transReference);
        public async Task<PlainResponse> PurchaseAirtime(AirTimeTopUpRequest requestModel, string userId)
        {
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

              /*  if (account.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Insufficient balance",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }*/

                return await _vasService.PurchaseAirtime(requestModel);
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

              /*  if (account.CurrentBalance < decimal.Parse(requestModel.amount))
                {
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "Insufficient balance",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }*/

                return await _vasService.PurchaseDataPlan(requestModel);
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
                var user = _appDbContext.Users.FirstOrDefault(acc => acc.UserUid.ToString() == requestModel.customerId);
                if (user == null)
                {
                    return new PlainResponse
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = "User's account does not exist",
                        ResponseCode = StatusCodes.Status400BadRequest
                    };
                }

                var account = _appDbContext.Accounts.FirstOrDefault(acc => acc.UserUid.ToString() == requestModel.customerId);
                if (account?.CurrentBalance < decimal.Parse(requestModel.amount))
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
                requestModel.customerName = $"{user.FirstName} {user.LastName}";
                requestModel.debitAccount = account?.AccountNumber;


                return await _vasService.InitBillerPayment(requestModel);
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
