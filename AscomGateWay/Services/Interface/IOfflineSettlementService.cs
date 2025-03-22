using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;

namespace AscomPayPG.Services.Interface
{
    public interface IOfflineSettlementService
    {
        public Task<ApiBaseResponse<OfflineSettlementResponseDTO>> ProcessOfflineSettlement(OfflinePaymentSettlementRequestDTO requestModel, string userId);
    }
}
