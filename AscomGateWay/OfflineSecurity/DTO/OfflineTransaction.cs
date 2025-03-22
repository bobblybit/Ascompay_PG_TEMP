namespace AscomPayPG.OfflineSecurity.DTO
{
    public class OfflineTransaction
    {
        public decimal TransactionsAmount { get; set; }
        public string Description { get; set; }
        public string TransactionId { get; set; }
        public string SenderWalletId { get; set; }
        public string SenderUid { get; set; }
        public string ReceiverWalletId { get; set; }
        public bool TransactionStatus { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public int SenderIntegerId { get; set; }

        public string SenderWalletName { get; set; }
        public string ReceiverWalletName { get; set; }
        public string ReceiverName { get; set; }
        public string SenderName { get; set; }
        public string ReceiverUid { get; set; }
    }
}
