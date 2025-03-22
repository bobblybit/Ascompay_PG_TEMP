namespace AscomPayPG.Models.VasModels
{
    public class AuthenticationRequestModeL
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class AirTimeTopUpRequest
    {
        public string phoneNumber { get; set; }
        public string network { get; set; }
        public string amount { get; set; }
        public string debitAccount { get; set; }
        public string transactionReference { get; set; }
    }
    public class DataTopUpRequest
    {
        public string phoneNumber { get; set; }
        public string amount { get; set; }
        public string debitAccount { get; set; }
        public string network { get; set; }
        public string productId { get; set; }
        public string transactionReference { get; set; }
    }
    public class ValidateBillerInputRequest
    {
        public string customerId { get; set; }
        public string amount { get; set; }
        public string billerId { get; set; }
        public string itemId { get; set; }

    }
    public class InitaiteBillPaymentRequest
    {
        public string customerId { get; set; }
        public string billerId { get; set; }
        public string? customerPhone { get; set; }
        public string? customerName { get; set; }
        public string itemId { get; set; }
        public string otherField { get; set; }
        public string? debitAccount { get; set; }
        public string amount { get; set; }
        public string? transactionReference { get; set; }
    }
}
