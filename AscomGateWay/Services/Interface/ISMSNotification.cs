using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;

namespace AscomPayPG.Services.Interface
{
    public interface ISMSNotification
    {
        Task<PlainResponse> SendSmsUsingBulkSms(string recipient, string message);
        Task<PlainResponse> SendSmsUsingMyKudiSms(string recipient, string message);
        Task<string> GetMessage(TransactionNotificationDTO transactionNotificationDTO, string smsType);
    }
}
