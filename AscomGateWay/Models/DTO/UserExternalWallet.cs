using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.DTO
{

    public class UserExternalWallet
    {
        [Key]
        public long ExternalWalletId { get; set; }
        public string UserUId { get; set; } //internal usage
        public string name { get; set; }
        public string number { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string phoneNo { get; set; }
        public string lastName { get; set; }
        public string tier { get; set; }
        public string productCode { get; set; }
        public string firstName { get; set; }
        public string maximumBalance { get; set; }
        public string ledgerBalance { get; set; }
        public string nuban { get; set; }
        public string bvn { get; set; }
        public string availableBalance { get; set; }
        public string totalWalletBalance { get; set; } //internal usage
        public string maximumDeposit { get; set; }
        public string freezeStatus { get; set; }
        public string lienStatus { get; set; }
        public string pndstatus { get; set; }
        public DateTime? LastUpdated { get; set; } = DateTime.Now;
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public bool IsDeprecated { get; set; }
    }
}