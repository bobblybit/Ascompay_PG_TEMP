namespace AscomPayPG.Models.WAAS
{
    public class WalletUpgradeTier3Request
    {
        public string? accountNumber { get; set; }
        public string? proofOfAddressVerification { get; set; }
        public string? bvn { get; set; }
        public string? nin { get; set; }

    }
}
