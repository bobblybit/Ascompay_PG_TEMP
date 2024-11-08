namespace AscomPayPG.Models.DTO
{
    public class TransferRequestDTO
    {
        public string? ReceiverAccountOrWallet { get; set; }
        public string ? SenderAccountOrWallet { get; set; }
        public string ?TransactionType { get; set; }
        public string? transactionId { get; set; }
        public string? ReceiverAccountName { get; set; }
        public decimal Amount { get; set; }
        public string Decription { get; set; }
    }
}
