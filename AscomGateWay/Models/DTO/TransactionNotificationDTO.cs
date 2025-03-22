namespace AscomPayPG.Models.DTO
{
    public class TransactionNotificationDTO
    {
        public string AccountNumber { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Balance { get; set; }
    }
}
