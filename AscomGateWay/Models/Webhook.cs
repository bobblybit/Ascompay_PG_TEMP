using AscomPayPG.Models.Shared;

namespace AscomPayPG.Models
{
    public class Webhook
    {
        public Webhook()
        {
            Reference = string.Empty;

            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public long WebhookId { get; set; }
        public string Reference { get; set; }
        public Guid Uid { get; set; } = new Guid();
        public string RequestString { get; set; }
        public string? EventType { get; set; }
        public string? Vendor { get; set; }
        public string? Service { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}