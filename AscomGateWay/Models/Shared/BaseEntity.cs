namespace AscomPayPG.Models.Shared
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeprecated { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; }
    }
}
