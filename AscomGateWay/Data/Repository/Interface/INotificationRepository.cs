using AscomPayPG.Models.DTOs;

namespace AscomPayPG.Data.Repository.Interface
{
    public interface INotificationRepository
    {
        Task<bool> AddNotification(NotificationLog notificationLog);
    }
}
