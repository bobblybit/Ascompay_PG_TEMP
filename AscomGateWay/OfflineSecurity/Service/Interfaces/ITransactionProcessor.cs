using AscomPayPG.OfflineSecurity.DTO;
using AscomPayPG.OfflineSecurity.Models;

namespace AscomPayPG.OfflineSecurity.Service.Interfaces;
public interface ITransactionProcessor
{
    public Task<ProcessingResult> ProcessTransactionRequest(OfflineTransactionRequest request);
}
