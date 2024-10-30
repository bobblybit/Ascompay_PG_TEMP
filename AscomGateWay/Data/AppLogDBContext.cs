using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace AscomPayPG.Data
{
    public class AppLogDBContext : DbContext
    {
        public AppLogDBContext(DbContextOptions<AppLogDBContext> options) : base(options) { }

        public DbSet<NotificationLog> Notification_Log { get; set; }
    }
}
