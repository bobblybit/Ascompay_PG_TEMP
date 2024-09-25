using AscomPayPG.Data.Repository.Interface;
using AscomPayPG.Models.DTOs;

namespace AscomPayPG.Data.Repository.Implementation
{
    public class NotificationRepository : INotificationRepository
    {
        private AppLogDBContext _AppLogDBContext;
        public NotificationRepository(AppLogDBContext appLogDBContext)
        {
            _AppLogDBContext = appLogDBContext;
        }
        public async Task<bool> AddNotification(NotificationLog notificationLog)
        {
            _AppLogDBContext.Notification_Log.Add(notificationLog);
            return _AppLogDBContext.SaveChanges() > 0;
        }
    }
}
