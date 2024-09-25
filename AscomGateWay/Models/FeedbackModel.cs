using AscomPayPG.Models.Shared;


namespace AscomPayPG.Models
{
    public class FeedbackModel : BaseEntity
    {
        public string Message { get; set; }
        public string? UserUid { get; set; }
        public string? UserEmail { get; set; }
        public string? Attachment { get; set; }
        public string FeedbackType { get; set; }
        public string Status { get; set; }
    }
}
