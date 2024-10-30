using AscomPayPG.Models.Shared;
using AscomPayPG.Models.VasModels;

namespace AscomPayPG.Services.Interface
{
    public interface IVasService
    {
        #region TOP UP
       Task<PlainResponse> PurchaseAirtime(AirTimeTopUpRequest requestModel, string userId);
       Task<PlainResponse> PurchaseData(DataTopUpRequest airtime, string userId);
       Task<PlainResponse> GetPhoneNumberNetwork(string phoneNumber);
       Task<PlainResponse> GetDataPlans(string phoneNumber);
       Task<PlainResponse> GetTopUpStatus(string transReference);
        #endregion

        #region BILLER
        Task<PlainResponse> GetCategory();
        Task<PlainResponse> GetCategoryBiller(string categoryId);
        Task<PlainResponse> GetBillerInputFields(string billerId);
        Task<PlainResponse> GetBillerPaymentStatus(string transactionReferenceId);
        Task<PlainResponse> VaildateBillerInputFields(ValidateBillerInputRequest requestModel);
        Task<PlainResponse> InitBillerPayment(InitaiteBillPaymentRequest requestModel);
        #endregion
    }
}
