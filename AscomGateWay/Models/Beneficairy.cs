

using AscomPayPG.Models.Shared;

namespace AscomPayPG.Models
{
    public class Beneficairy : BaseEntity
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string? Bank { get; set; }
        public string? BankCode { get; set; }
        public string UserId { get; set; }
        public bool IsInternal { get; set; }
    }
}
