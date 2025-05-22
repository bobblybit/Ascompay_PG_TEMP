using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Data.Repository.Interface;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using AscomPayPG.Services.Implementation;
using AscomPayPG.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AscomPayPG.Helpers
{
    public class TransactionHelper : ITransactionHelper
    {
        private readonly AppDbContext _appContext;
        private static List<TransactionType> _transactionTypes;
        private readonly IEmailNotification _emailNotification;
        private readonly ISMSNotification _smsNotification;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TransactionHelper(AppDbContext appDbContext, 
                                 IEmailNotification emailNotification,
                                 INotificationRepository notificationRepository,
                                 ISMSNotification smsNotification,
                                 IHttpContextAccessor httpContextAccessor)
        {
            _appContext = appDbContext;
            _emailNotification = emailNotification;
            _notificationRepository = notificationRepository;
            _smsNotification = smsNotification;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<decimal> CalculateVAT(decimal amount, string transactionType)
        {
            var transactionTypeDetails = _appContext.TransactionType.FirstOrDefault(x => x.Ttype
                                                                             .Replace(" ", "")
                                                                             .Replace("(", "")
                                                                             .Replace(")", "") == transactionType);

           return  Math.Round((decimal)transactionTypeDetails.T_Vat / 100 * amount, 1);
        } 
       
        public async Task<decimal> CalculateCharges(decimal amount, string transactionType)
        {
            var transactionTypeDetails = _appContext.TransactionType.FirstOrDefault( x => x.Ttype
                                                                                      .Replace(" ", "")
                                                                                      .Replace("(","")
                                                                                      .Replace(")","") == transactionType);
            if (transactionTypeDetails != null)
            {

             if (transactionTypeDetails.By_Percent)
                return Math.Round((decimal)transactionTypeDetails.T_Percentage / 100 * amount, 1);
             else
                return transactionTypeDetails.T_Amount;
            }
            return 0;
        }

        public async Task<decimal> GetPaymentProviderCharges(string transactionType)
        {
            var charges = await _appContext.TransactionType.FirstOrDefaultAsync(x => x.Ttype.Replace(" ", "").Replace("(", "").Replace(")", "") == transactionType);
            return charges == null ? 0 : charges.T_Provider_Charges;
        }
        public async Task NotifyForCredit(string receiverFullName, string receiverEmail, string sender, string amount, string balance, string transactionTime, string decription)
        {
            string errorMessage = string.Empty;
            var emailObject = new TransactionAlertNotificationDTO
            {
                Amount = amount,
                Balance = balance,
                User = receiverFullName,
                Sender = sender,
                Time = transactionTime,
                Description = decription
            };

            var htmlContent = EmailComposer.ComposeCreditOrDebitNotificationHtmlContent(emailObject, true);
            try
            {
                await _emailNotification.NewSendEmail("Ascom Pay", "Transaction Notification", htmlContent, receiverEmail);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                //log notification

                throw;
            }
            finally
            {
              /*  var notificationToAdd = new NotificationLog
                {
                    DCreatedOn = DateTime.Now,
                    DLastTriedOn = DateTime.Now,
                    IHasAttachment = false,
                    ITryCount = 5,
                    SRecipient = receiverEmail,
                    SMessage = htmlContent,
                    Sender = "Ascom",
                    SComment = errorMessage == string.Empty ? "Sent" : $"Not sent {errorMessage}",
                    SSubject = "Transaction Notification",
                    IStatus = 1,
                    SAttachmentCount = 0,
                    Origin = "AscomPayPG",
                };

                var responsed = await _notificationRepository.AddNotification(notificationToAdd);*/
            }
        }

        public void NotifyForDebit(string mailReciever, string receiverFullName, string amount, string balance, string vat, string charges, string transactionTime, string description, string transactionReference)
        {
            var errorMessage = string.Empty;
            var emailObject = new TransactionAlertNotificationDTO
            {
                Amount = amount,
                Balance = balance,
                User = receiverFullName,
                Time = transactionTime,
                Vat = vat,
                Charges = charges,
                Description = description,
                Reference = transactionReference
            };
            var htmlContent = EmailComposer.ComposeCreditOrDebitNotificationHtmlContent(emailObject, false);
            try
            {
                _emailNotification.NewSendEmail("Ascom Pay", "Transaction Notification", htmlContent, mailReciever);
            }
            catch (Exception ex)
            {
                //log notification
                errorMessage = ex.Message;
                throw;
            }
            finally
            {
                var notificationToAdd = new NotificationLog
                {
                    DCreatedOn = DateTime.Now,
                    DLastTriedOn = DateTime.Now,
                    IHasAttachment = false,
                    ITryCount = 5,
                    SRecipient = mailReciever,
                    SMessage = htmlContent,
                    Sender = "Ascom",
                    SComment = errorMessage == string.Empty ? "Sent" : $"Not sent {errorMessage}",
                    SSubject = "Transaction Notification",
                    IStatus = 1,
                    SAttachmentCount = 0,
                    Origin = "AscomPayPG",
                };
                _notificationRepository.AddNotification(notificationToAdd);
            }
        }

        public async Task NotifyForCreditSMS(User appUser, string receiverAccount, string amount, string balance, string description)
        {
            var errorMessage = string.Empty;
            TransactionNotificationDTO transactionNotificationDTO = new TransactionNotificationDTO()
            {
                AccountNumber = receiverAccount.ToString(),
                Amount = amount.ToString(),
                Balance = balance.ToString(),
                Description = description,
                Date = DateTime.Now
            };
            string sms = await _smsNotification.GetMessage(transactionNotificationDTO, TransactionTypes.Credit.ToString().ToLower());
            try
            {
                if (!string.IsNullOrEmpty(sms))
                {
                    //mykudisms
                    PlainResponse respsms = await _smsNotification.SendSmsUsingMyKudiSms(appUser.PhoneNumber.Trim(), sms);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                //log notification

                throw;
            }
        }

        public async Task NotifyForDebitSMS(User appUser, string sourceAccount, string amount, string balance, string description)
        {
                var errorMessage = string.Empty;
                TransactionNotificationDTO transactionNotificationDTO = new TransactionNotificationDTO()
                {
                    AccountNumber = sourceAccount.ToString(),
                    Amount = amount.ToString(),
                    Balance = balance.ToString(),
                    Description = description,
                    Date = DateTime.Now
                };
                string sms = await _smsNotification.GetMessage(transactionNotificationDTO, TransactionTypes.Withdrawal.ToString().ToLower());
                try
                {
                    if (!string.IsNullOrEmpty(sms))
                    {
                        //mykudisms
                        PlainResponse respsms = await _smsNotification.SendSmsUsingMyKudiSms(appUser.PhoneNumber.Trim(), sms);
                    }
                }
                catch (Exception ex)
                {
                    //log notification
                    errorMessage = ex.Message;
                    throw;
                }
        }

        public async Task<string> GetAccessToken(string payload, string requestUrl, string xToken)
        {

            if (xToken == null)
                return null;
            string iP = SmartObj.GetServerLocalIp();
            if (iP == null)
                return null;

            string generatedDate = DateTime.Now.ToString();
            SmartObj.CreatePasswordHash($"{xToken}X{iP}X{generatedDate}", out byte[] accessTokenHash, out byte[] accessTokenSalt);
            SmartObj.CreatePasswordHash($"{payload}", out byte[] payloadHash, out byte[] payloadSalt);

            var log = new PaymentGateWayMiddlewareLog
            {
                AccessToken = Convert.ToBase64String(accessTokenHash),
                RquestUrl = requestUrl,
                GenerationDate = generatedDate,
                AccessTokenHash = accessTokenHash,
                AccessTokenSalt = accessTokenSalt,
                PayloadHash = payloadHash,
                PayloadSalt = payloadSalt,
                Ip = iP
            };

            _appContext.PaymentGateWayMiddlewareLogs.Add(log);


            if (!(_appContext.SaveChanges() > 0))
                return null;

            return Convert.ToBase64String(accessTokenHash);
        }
        public async Task AddHeaders(HttpClient httpClient, string payloadLoadAsJsonString, string fullUrl, string _xToken = "")
        {
            var bearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var xtoken = !string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Headers["Xtoken"].ToString()) ? _httpContextAccessor.HttpContext.Request.Headers["Xtoken"].ToString() : _xToken;
            var accessToken = await GetAccessToken(payloadLoadAsJsonString, fullUrl, xtoken);

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("CrossAccess", $"{accessToken}");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"{bearer}");
            httpClient.DefaultRequestHeaders.Add("Xtoken", $"{xtoken}");
        }

        async Task<Dictionary<string, string>> ITransactionHelper.GetPGMRequestHeaders(string payloadLoadAsJsonString, string fullUrl, string _xToken)
        {
            var bearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var xtoken = !string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Headers["Xtoken"].ToString()) ? _httpContextAccessor.HttpContext.Request.Headers["Xtoken"].ToString() : _xToken;
            var accessToken = await GetAccessToken(payloadLoadAsJsonString, fullUrl, xtoken);

            var PGMHeaders = new Dictionary<string, string>
            {
                { "CrossAccess", accessToken },
                { "Authorization", bearer },
                { "Xtoken", xtoken }
            };

            return PGMHeaders;
        }
    }
}

