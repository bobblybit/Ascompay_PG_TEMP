
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;

namespace AscomPayPG.Services
{
    public interface ITransactionsRepository<T> : IRepository<T>
    {

        public Task<TransactionsResponse> GetPaginatedAll(int page = 1);
        public Task<TransactionsResponse> GetPaginatedAll(string uid, int page = 1);
        public Task<Transactions> GetPayRequest(string requestId);
        public Task<bool> IsRequestTransactionIdAvailable(string requestId);
        public Task<Tuple<bool, string, string>> UpdateUserAccount(long itemId);

    }
}