using AscomPayPG.Models;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.ViewModels;

namespace AscomPayPG.Services
{
    public interface IPayment
    {
        public Task<PaymentResponseView> Pay(GatewayViewModel payReq, Transactions dbTransactions, PaymentGateway selectedGateway);
        public Task<PayQueryResponse> PayQuery(string queryReq, PaymentGateway selectedGateway);
    }

}