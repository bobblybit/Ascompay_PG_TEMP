using AscomPayPG.Models.DTO;

namespace AscomPayPG.Data.Repository.Interface
{
    public interface INotificationRepository
    {
        Task<bool> AddNotification(NotificationLog notificationLog);
    }
}
