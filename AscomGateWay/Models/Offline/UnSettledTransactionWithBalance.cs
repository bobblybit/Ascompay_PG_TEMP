using AscomPayPG.OfflineSecurity.DTO;

namespace AscomPayPG.Models.Offline
{
    public class UnSettledTransactionWithBalance
    {
        public decimal Balance { get; set; }
        public List<OfflineTransaction> UnSettledTransaction { get; set; } = new List<OfflineTransaction>();
    }
}
