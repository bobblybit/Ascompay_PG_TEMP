namespace AscomPayPG.Models
{
    public class UserSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RereshToken { get; set; }
        public bool IsActive { get; set; }
        public string? LoginChannel { get; set; }
        public DateTime TokenExpiry { get; set; }
        public DateTime RereshTokenExpiry { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
