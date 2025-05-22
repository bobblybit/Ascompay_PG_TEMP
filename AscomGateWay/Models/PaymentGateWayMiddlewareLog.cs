namespace AscomPayPG.Models
{
    public class PaymentGateWayMiddlewareLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public byte [] PayloadHash { get; set; }
        public byte[] PayloadSalt { get; set; }
        public byte[] AccessTokenSalt { get; set; }
        public byte[] AccessTokenHash { get; set; }
        public string GenerationDate { get; set; }
        public string RquestUrl { get; set; }
        public string AccessToken { get; set; }
        public string Ip { get; set; }
        public bool IsUsed { get; set; }
    }
}
