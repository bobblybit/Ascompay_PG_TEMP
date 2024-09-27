
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;

namespace AscomPayPG.Services
{
    public interface IClientRequestRepository<T> : IRepository<T>
    {
        public Task<ClientRequestResponse> GetPaginatedAll(int page = 1);
        public Task<ClientRequest> GetClientReference(string reference);
        public Task<User> GetUser(string uid);
        public Task<UserWallet> GetUserWallet(string destination);
        public Task<Account> GetUserAccount(string destination);
        public Task<Account> GetUserAccountByUserUid(string userUid);
        public Task<UserWallet> GetWalletById(string Id);
        public Task<bool> UpdateWallet(UserWallet model);
        Task<bool> UpdateExternalWallet(UserExternalWallet model);
        public Task<bool> UpdateAccount(Account model);
        public Task<bool> AddTransaction(Transactions model);

    }
}