
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace AscomPayPG.Models
{
    public class UserDeviceAuthentication : BaseEntity
    {
        [ForeignKey("UserId")]
        public long UserId { get; set; }
        public User ? User { get; set; }
        public string? AuthenticationKey { get; set; }
    }
}
