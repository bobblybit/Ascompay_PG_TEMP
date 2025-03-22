using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using DB_MODALS.Entities;

namespace AscomPayPG.Services.Interface
{
    public interface IAccountService
    {
        Task<ApiBaseResponse<bool>> UpdateTierAccountUpgradeStatus(string accountUpgradeId, string newStatus, string declineReason = "");
        Task<ApiBaseResponse<bool>> UpdateTier3AccountUpgradeStatus(string newStatus, AccountUpgrade accountUpgrade, string declineReason = "");
        Task<ApiBaseResponse<bool>> UpdateTier2AccountUpgradeStatus(string newStatus, AccountUpgrade accountUpgrade, string declineReason = "");
    }
}
