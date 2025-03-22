using AscomPayPG.OfflineSecurity.DTO;

namespace AscomPayPG.Models.DTO
{
    public class OfflinePaymentSettlementRequestDTO
    {
        public List<OfflineTransaction> OfflineTransactions { get; set; }
    }
}
