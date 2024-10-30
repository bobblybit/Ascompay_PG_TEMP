using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.DTO
{
    public class NotificationLog
    {
        [Key]
        public long NlogId { get; set; }

        public string? Origin { get; set; }

        public string? Sender { get; set; }

        public string? INotificationType { get; set; }

        public string? SRecipient { get; set; }

        public string? SCc { get; set; }

        public string? SMessage { get; set; }

        public string? SSubject { get; set; }

        public bool? IHasAttachment { get; set; }

        public int? SAttachmentCount { get; set; }

        public int? IStatus { get; set; }

        public int? ITryCount { get; set; }

        public DateTime? DLastTriedOn { get; set; }

        public DateTime? DCreatedOn { get; set; }

        public string? SComment { get; set; }
    }
}
