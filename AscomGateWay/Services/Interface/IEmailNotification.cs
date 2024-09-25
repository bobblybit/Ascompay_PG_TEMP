using System.Collections.Generic;
using System.Threading.Tasks;
using DB_MODALS.Entities;

namespace SERVICES.Services.Interface.Notification;

public interface IEmailNotification
{
	Task<bool> SendEmail(string DisplayName, string Subject, string HTMLMessage, List<string> Receivers);
	Task<bool> NewSendEmail(string DisplayName, string Subject, string HTMLMessage, string Receiver);
	Task<bool> AddNewEmailConfiguration(SMTPConfiguration sMTPConfiguration);
}
