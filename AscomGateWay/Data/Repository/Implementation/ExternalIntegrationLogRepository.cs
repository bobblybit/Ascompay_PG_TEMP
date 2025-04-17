using AscomPayPG.Data.Repository.Interface;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;

namespace AscomPayPG.Data.Repository.Implementation
{
    public class ExternalIntegrationLogRepository : IExternalIntegrationLogRepository
    {
        private AppDbContext _appDbContext;
        public ExternalIntegrationLogRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task AddExternalIntegrationLog(ExternalIntegrationLog externalIntegrationLog)
        {
            _appDbContext.ExternalIntegrationLogs.Add(externalIntegrationLog);
            _appDbContext.SaveChanges();
        }

        public async Task SaveAccountLookUp(AccountLookUpLog accountLookUpLog)
        {
            _appDbContext.AccountLookUpLog.Add(accountLookUpLog);
            _appDbContext.SaveChanges();
        }
    }
}
