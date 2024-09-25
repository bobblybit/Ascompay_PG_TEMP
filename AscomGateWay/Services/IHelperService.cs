using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.ViewModels;

namespace AscomPayPG.Services
{
    public interface IHelperService
    {
        Task CustomLogError(Exception e, string action);
        public Task<string> GetConfigItem(string configName);
        public Task<string?> GetNewReferenceId();
        public Task<ResponseMessage> SessionValidation(string accessToken);
        public Transactions GetPaymentDefaultModel(PaymentRequest payReq, string reference, string description);
        //public Task<Transactions> GetPaymentUpdateModel(GatewayViewModel payReq);
        public Task<AppResult<string>> GetResource(string url, Dictionary<string, string> headers, Dictionary<string, string> parameters = null);
        public Task<AppResult<string>> PostResource(string url, Dictionary<string, string> headers, object data, Dictionary<string, string> parameters = null);
        public string Base64Encode(string plainText);
        public string Base64Decode(string base64EncodedData);
        public TransactionType GetOneTransactionType(int itemId);

    }
}