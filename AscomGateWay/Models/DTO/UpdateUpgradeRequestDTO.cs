namespace AscomPayPG.Models.DTO
{
    public class UpdateUpgradeRequestDTO
    {
        public string NewStatus { get; set; }
        public string RequestId { get; set; }
        public string? RejectionComment { get; set; }
    }
}
