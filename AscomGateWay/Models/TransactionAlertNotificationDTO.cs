namespace AscomPayPG.Models
{
    public class TransactionAlertNotificationDTO
    {
        //user  amount description time reference currentdate balance

        public string User { get; set; }
        public string Sender { get; set; }
        public string Vat { get; set; }
        public string Charges { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
        public string Reference { get; set; }
        public string Currentdate { get; set; }
        public string Balance { get; set; }
    }
}
