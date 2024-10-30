using AscomPayPG.Models.Shared;
using AscomPayPG.Models.VasModels;

namespace AscomPayPG.Services.Gateways.Interface
{
    public interface I9psbVaS
    {
        #region TOP-UP
        Task<PlainResponse> GetPhoneNetwork(string phoneNumber);
        Task<PlainResponse> PurchaseAirtime(AirTimeTopUpRequest requestModel);
        Task<PlainResponse> GetDataPlans(string phoneNumber);
        Task<PlainResponse> PurchaseDataPlan(DataTopUpRequest requestModel);
        Task<PlainResponse> GetTopUpStatus(string transactionReferenceId);
        #endregion

        #region BILLER
        Task<PlainResponse> GetCategory();
        Task<PlainResponse> GetCategoryBiller(string categoryId);
        Task<PlainResponse> GetBillerInputFields(string billerId);
        Task<PlainResponse> VaildateBillerInputFields(ValidateBillerInputRequest requestModel);
        Task<PlainResponse> InitBillerPayment(InitaiteBillPaymentRequest requestModel);
        Task<PlainResponse> GetBillerPaymentStatus(string transactionReferenceId);
        #endregion
    }
}
