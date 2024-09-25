
using AscomPayPG.Models.Shared;

namespace AscomPayPG.Models
{
    public class WebhookModel : BaseEntity
    {
        public string VirtualAcctReference { get; set; }
        public string Reference { get; set; }
        public string Event { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
    }
}
