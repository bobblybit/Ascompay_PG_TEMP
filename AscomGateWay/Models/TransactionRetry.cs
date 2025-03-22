namespace AscomPayPG.Models
{
    public class TransactionRetry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public bool IsOffline { get; set; }
        public bool HasbeenSettled { get; set; }
        public string? ReceiverAccountOrWallet { get; set; }
        public string? SenderAccountOrWallet { get; set; }
        public string? ReceiverWalletId { get; set; }
        public string? SenderWalletId { get; set; }
        public bool hasPostedSuccessfully { get; set; }
        public string? TransactionType { get; set; }
        public string? transactionId { get; set; }
        public string? ReceiverAccountName { get; set; }
        public decimal Amount { get; set; }
        public string Decription { get; set; }
        public string OfflineId { get; set; }


    }
}
