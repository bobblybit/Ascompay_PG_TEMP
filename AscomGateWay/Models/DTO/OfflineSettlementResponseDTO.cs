using AscomPayPG.OfflineSecurity.DTO;

namespace AscomPayPG.Models.DTO
{
    public class OfflineSettlementResponseDTO
    {
        public int TotalSettledTransaction { get; set; }
        public int TotalUnsettledTransaction { get; set; }
        public decimal NewBalance { get; set; }
        public List<OfflineTransaction> UnsettledTransactions { get; set; }
        public byte[] EncryptedWallets { get; set; } = new byte[16];
        public byte[] key { get; set; } = new byte[16];
        public byte[] Signature { get; set; } = new byte[16];
    }
}
