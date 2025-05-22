using AscomPayPG.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WebhookId { get; set; }
        public string Reference { get; set; }
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string RequestString { get; set; }
        public string? EventType { get; set; }
        public string? Vendor { get; set; }
        public string? Service { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsSettled { get; set; }
    }
}