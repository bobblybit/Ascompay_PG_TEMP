using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AscomPayPG.Models.DTO
{

    public class UserWallet
    {
        [Key]
        public long WalletId { get; set; }
        public Guid WalletUID { get; set; } = Guid.NewGuid(); // or a alpha numeric hash ID

        public string? WalletName { get; set; }
        public long UserId { get; set; }
        public Guid? UserUid { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? PrevioseBalance { get; set; } = 0;

        public virtual User? User { get; set; }
        public DateTime? LastUpdated { get; set; } = DateTime.Now;
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public bool IsDeprecated { get; set; }
        [ForeignKey("WalletTypeId")]
        public Guid WalletTypeId { get; set; }
        public WalletType? WalletType { get; set; }
    }
}