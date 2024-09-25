using AscomPayPG.Models.Shared;
using AscomPayPG.Models.ViewModels;

namespace AscomPayPG.Services
{

    public interface IPaymentProcessor
    {
        public Task<AppResult<GatewayViewModel>> Pay(PaymentRequest payReq);
        public Task<AppResult<PaymentResponseView>> PayInit(GatewayViewModel payInitReq);
        public Task<PayQueryResponse> QueryStatus(string queryReq);
        public Task<PayStatusViewModel> ProcessCallBack(string TransactionsReference);
        public Task<GatewayViewModel> GetGatewayViewData(string reference);
    }
}