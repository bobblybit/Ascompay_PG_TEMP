using AscomPayPG.Models.Shared;

namespace AscomPayPG.Models
{
    public class ClientRequest
    {
        public ClientRequest()
        {
            Status = false;
            StatusDescription = ClientRequestEnum.Init.ToString();
            CallbackURL = string.Empty;
            Reference = string.Empty;

            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public long ClientRequestId { get; set; }
        public string Reference { get; set; }
        public Guid Uid { get; set; }
        public bool Status { get; set; }
        public string? StatusDescription { get; set; }
        public string CallbackURL { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}