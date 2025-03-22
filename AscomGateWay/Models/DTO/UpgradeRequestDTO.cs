namespace AscomPayPG.Models.DTO
{
    public class UpgradeRequestDTO
    {
        public string UserId { get; set; }
        public string AccountNumber { get; set; }
        public string UpgradeT0 { get; set; }
        public IFormFile? ProofAddressURl { get; set; }
    }
}
