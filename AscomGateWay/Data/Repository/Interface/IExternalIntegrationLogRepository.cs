using AscomPayPG.Models;
using AscomPayPG.Models.DTO;

namespace AscomPayPG.Data.Repository.Interface
{
    public interface IExternalIntegrationLogRepository
    {
        Task AddExternalIntegrationLog(ExternalIntegrationLog externalIntegrationLog);
    }
}
