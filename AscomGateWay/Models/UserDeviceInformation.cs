using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace AscomPayPG.Models
{
    public class UserDeviceInformation : BaseEntity
    {
        [ForeignKey("UserId")]
        public long UserId { get; set; }
        public User ? User { get; set; }
        public string DeviceModel { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public bool IsPrimaryDevice { get; set; }
        //public string? AuthenticationKey { get; set; }
    }
}
