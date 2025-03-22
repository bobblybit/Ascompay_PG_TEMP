using AscomPayPG.Models.Shared;

namespace DB_MODALS.Entities
{
    public class AccountUpgrade:BaseEntity
    {
        public string UserId { get; set; }
        public string? AccountNumber { get; set; }
        public bool IsApproved { get; set; }
        public string? ProofAddressURl { get; set; }
        public string? UpgradeT0 { get; set; }
        public string? Status { get; set; }
        public string? NIN { get; set; }
        public string? Bvn { get; set; }
        public string? AccountName { get; set; }
        public string? ChannelType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? UserPhoto { get; set; }
        public string? IdType { get; set; }
        public string? IdNumber { get; set; }
        public DateTime? IdIssueDate { get; set; }
        public DateTime? IdExpiryDate { get; set; }
        public string? IdCardFront { get; set; }
        public string? IdCardBack { get; set; }
        public string? HouseNumber { get; set; }
        public string? StreetName { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? LocalGovernment { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? Pep { get; set; }
        public string? CustomerSignature { get; set; }
        public string? UtilityBill { get; set; }
        public string? NearestLandmark { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? DeclineReason { get; set; }
    }
}
