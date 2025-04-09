using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using System;

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
        Task<User> GetUserBySessionAsync(string session);
        Task<AccountLookUpLog> GetLookUpLog(string lookUpId);
        public Task<bool> ValidateTransaction(string accessToken, string senderAccount, string receiverAccount, decimal amount, string transactionType);
        public Task<UserSession> GetUserCurrentSessionAsync(string sessionToken, string refreshToken);
        
    }
}