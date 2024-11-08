using AscomPayPG.Models;
using AscomPayPG.Models.DTO;

namespace AscomPayPG.Services
{
    public interface IClientRequestRepository<T> : IRepository<T>
    {
        public Task<bool> RegisterTransaction(  
                                                  decimal NetAmount,
                                                  decimal ProviderCharges,
                                                  decimal MarchantCharges,
                                                  string recieverName,
                                                  User sender,
                                                  string transactionReference,
                                                  decimal amount,
                                                  string decription,
                                                  string trasactionType,
                                                  decimal vat,
                                                  decimal totalCharges,
                                                  string provider,
                                                  string senderAccount = "",
                                                  string recieverAccount = "",
                                                  string senderWallet = "",
                                                  string recieverWallet = "",
                                                  bool setAsCompleted = false
                                                  );
      
        public Task<ClientRequestResponse> GetPaginatedAll(int page = 1);
        public Task<ClientRequest> GetClientReference(string reference);
        public Task<User> GetUser(string uid);
        public Task<UserWallet> GetUserWallet(string destination);
        public Task<Account> GetUserAccount(string destination);
        public Task<Account> GetAccount(string destination);
        public Task<Account> GetUserAccountByUserUid(string userUid);
        public Task<UserWallet> GetWalletById(string Id);
        public Task<bool> UpdateWallet(UserWallet model);
        Task<bool> UpdateExternalWallet(UserExternalWallet model);
        public Task<bool> UpdateAccount(Account model);
        public Task<bool> AddTransaction(Transactions model);
        public Task<bool> UpdateTransactionStatusByReference(string referecneId, string newStatus);
    }
}