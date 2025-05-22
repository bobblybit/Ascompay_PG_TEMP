using AscomPayPG.Models.Shared;
using AscomPayPG.Models.VasModels;
using PaymentGateWayMiddleWare.Model;

namespace AscomPayPG.Services.Gateways.Interface
{
    public interface I9psbVaS
    {
        #region TOP-UP
        Task<Nine9psbGenResponse<PhoneNumberLookUpResponse>> GetPhoneNetwork(string phoneNumber);
        Task<Nine9psbListGenResponse<DataPlansResponse>> GetDataPlans(string phoneNumber);
        Task<Nine9psbGenResponse<AirtimeTopUp>> PurchaseAirtime(AirTimeTopUpRequest requestModel);
        Task<Nine9psbGenResponse<DataTopUpResponse>> PurchaseDataPlan(DataTopUpRequest requestModel);
        Task<Nine9psbGenResponse<StatusResponse>> GetTopUpStatus(string transactionReferenceId);
        #endregion

        #region BILLER
        Task<Nine9psbListGenResponse<Category>> GetCategory();
        Task<Nine9psbListGenResponse<CategoryBiller>> GetCategoryBiller(string categoryId);
        Task<Nine9psbListGenResponse<BillerField>> GetBillerInputFields(string billerId);
        Task<Nine9psbGenResponse<BillerFieldValidationResponse>> VaildateBillerInputFields(ValidateBillerInputRequest requestModel);
        Task<Nine9psbGenResponse<BillerPaymentResponse>> InitBillerPayment(InitaiteBillPaymentRequest requestModel);
        Task<Nine9psbGenResponse<TransactionStatusResponse>> GetBillerPaymentStatus(string transactionReferenceId);
        #endregion
    }
}
