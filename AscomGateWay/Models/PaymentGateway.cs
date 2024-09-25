using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models
{
    public class PaymentGateway
    {
        public PaymentGateway()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public long PaymentGatewayId { get; set; }
        public string? Name { get; set; }
        public string? PayUrl { get; set; }
        public string? QueryUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}