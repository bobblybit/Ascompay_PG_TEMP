namespace AscomPayPG.Models.WAAS
{
    public class WalletUpgradeTier2Request
    {
        public string accountNumber { get; set; }
        public string bvn { get; set; }
        public string accountName { get; set; }
        public string channelType { get; set; }
        public string phoneNumber { get; set; }
        public string tier { get; set; }
        public string email { get; set; }
        public string userPhoto { get; set; }
        public string idType { get; set; }
        public string idNumber { get; set; }
        public DateTime? idIssueDate { get; set; }
        public DateTime? idExpiryDate { get; set; }
        public string idCardFront { get; set; }
        public string idCardBack { get; set; }
        public string houseNumber { get; set; }
        public string streetName { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string localGovernment { get; set; }
        public String nin { get; set; }
        public string approvalStatus { get; set; }
        public string pep { get; set; }
        public string customerSignature { get; set; }
        public string utilityBill { get; set; }
        public string nearestLandmark { get; set; }
        public string placeOfBirth { get; set; }
        public string proofOfAddressVerification { get; set; }
    }
}
