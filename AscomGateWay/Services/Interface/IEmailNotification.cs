using AscomPayPG.Models.Notification;

namespace AscomPayPG.Services.Interface;

public interface IEmailNotification
{
    Task<bool> SendEmail(string DisplayName, string Subject, string HTMLMessage, List<string> Receivers);
    Task<bool> NewSendEmail(string DisplayName, string Subject, string HTMLMessage, string Receiver);
    Task<bool> AddNewEmailConfiguration(SMTPConfiguration sMTPConfiguration);
}
