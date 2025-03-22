using AscomPayPG.OfflineSecurity.Models;

namespace AscomPayPG.OfflineSecurity.Service.Interfaces;
public interface ITransactionRepository
{
    Task SaveTransaction(OfflineTransactionSec transaction);
    public Task<bool> IsNonceUsed(string nonce);
    public Task SaveNonce(string nonce);
    Task UpdateTransactionStatus(string transactionId, bool status);
}
