namespace AscomPayPG.Models.DTOs
{
    public class TransferRequestDTO
    {
        public string? ReceiverAccount { get; set; }
        public bool IsAccount { get; set; }
        public string?  transactionId { get; set; }
        public decimal Amount { get; set; }
        public string Decription { get; set; }
        public string SenderAccountOrWallet { get; set; }
    }
}
