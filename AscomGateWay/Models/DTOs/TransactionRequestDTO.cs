namespace AscomPayPG.Models.DTOs
{
    public class TransactionRequestDto
    {
        public string RecieverAccountId { get; set; }
        public bool IsAccount { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string SenderAccount { get; set; }
    }
}
