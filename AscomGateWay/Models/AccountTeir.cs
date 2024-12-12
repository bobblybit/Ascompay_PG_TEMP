using AscomPayPG.Models.Shared;

namespace AscomPayPG.Models
{
    public class AccountTeir : BaseEntity
    {
        public string Name { get; set; }
        public decimal MaxSingleTransfer { get; set; }
        public decimal MaxCumulativeBalance { get; set; }
    }
}
