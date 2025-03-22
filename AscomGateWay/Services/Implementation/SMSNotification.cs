using AscomPayPG.Data.Enum;
using AscomPayPG.Data.Repository.Interface;
using AscomPayPG.Helpers;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Notification;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Interface;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text;

namespace AscomPayPG.Services.Implementation
{
    public class SMSNotification : ISMSNotification
    {
        private readonly IConfiguration _configuration = ConfigurationHelper.GetConfigurationInstance();
        //private readonly AppdbContext _appcontext;
        private readonly IExternalIntegrationLogRepository _externalIntegrationLogRepository;

        public SMSNotification(IExternalIntegrationLogRepository externalIntegrationLogRepository)
        {
            _externalIntegrationLogRepository = externalIntegrationLogRepository;
        }
       
        public async Task<PlainResponse> SendSmsUsingBulkSms(string recipient, string message)
        {
            PlainResponse resp = new PlainResponse();

            try
            {
                string baseUrl = _configuration["BulkSms:BaseURL"];
                string callbackUrl = _configuration["BulkSms:CallbackUrl"];
                string apiToken = _configuration["BulkSms:ApiToken"];
                string gateway = _configuration["BulkSms:Gateway"];
                string from = _configuration["BulkSms:From"];
                BulkSmsRequestDTO bulkSmsRequestDTO = new BulkSmsRequestDTO()
                {
                    to = recipient,
                    body = message,
                    from = from,
                    api_token = apiToken,
                    gateway = gateway,
                    callback_url = callbackUrl,
                    customer_reference = $"{GenRefNo()}"
                };

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("api_token", $"{apiToken}");

                    var fullUrl = $"{baseUrl}";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(bulkSmsRequestDTO), Encoding.UTF8, "application/json");

                    var log = new ExternalIntegrationLog
                    {
                        CreatedBy = bulkSmsRequestDTO.to,
                        RequestTime = DateTime.Now,
                        RequestPayload = JsonConvert.SerializeObject(bulkSmsRequestDTO),
                        Service = "Notification",
                        Vendor = "BulkSMS"
                    };

                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        log.Response = apiResponse;
                        log.ResponseTime = DateTime.Now;

                         await _externalIntegrationLogRepository.AddExternalIntegrationLog(log);
                        if (response.IsSuccessStatusCode)
                        {
                            resp.IsSuccessful = true;
                            resp.ResponseCode = (int)response.StatusCode;
                            resp.Data = JsonConvert.DeserializeObject<BulkSmsResponseData>(apiResponse);
                            resp.Message = resp.Data.message;
                        }
                        else
                        {
                            resp.IsSuccessful = false;
                            resp.ResponseCode = (int)response.StatusCode;
                            resp.Data = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                        }
                        return resp;
                    }
                }

            }
            catch (Exception ex)
            {

                resp.IsSuccessful = false;
                resp.ResponseCode = StatusCodes.Status400BadRequest;
                resp.Message = ex.Message;
                return resp;
            }
        }

        public async Task<PlainResponse> SendSmsUsingMyKudiSms(string recipient, string message)
        {
            PlainResponse resp = new PlainResponse();

            try
            {
                string baseUrl = _configuration["MyKudisms:BaseURL"];
                string senderID = _configuration["MyKudisms:senderID"];
                string apiToken = _configuration["MyKudisms:ApiToken"];
                string trimLeadingZero = TrimLeadingZero(recipient);
                string recipientPhone = $"234{trimLeadingZero}";
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    //Add other fields
                    multipartFormContent.Add(new StringContent(apiToken), name: "token");
                    multipartFormContent.Add(new StringContent(senderID), name: "senderID");
                    multipartFormContent.Add(new StringContent(recipientPhone), name: "recipients");
                    multipartFormContent.Add(new StringContent(message), name: "message");

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        var fullUrl = $"{baseUrl}";

                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = recipient,
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(multipartFormContent),
                            Service = "Notification",
                            Vendor = "MyKudiSMS"
                        };

                        using (var response = await httpClient.PostAsync(fullUrl, multipartFormContent))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();

                            log.Response = apiResponse;
                            log.ResponseTime = DateTime.Now;

                            await _externalIntegrationLogRepository.AddExternalIntegrationLog(log);
                            if (response.IsSuccessStatusCode)
                            {
                                resp.IsSuccessful = true;
                                resp.ResponseCode = (int)response.StatusCode;
                                resp.Data = JsonConvert.DeserializeObject<MyKudiSmsResponseDTO>(apiResponse);
                                resp.Message = resp.Data.msg;
                            }
                            else
                            {
                                resp.IsSuccessful = false;
                                resp.ResponseCode = (int)response.StatusCode;
                                resp.Data = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            }
                            return resp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                resp.IsSuccessful = false;
                resp.ResponseCode = StatusCodes.Status400BadRequest;
                resp.Message = ex.Message;
                return resp;
            }
        }

        public string TrimLeadingZero(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.StartsWith("0"))
            {
                return phoneNumber.Substring(1);
            }
            return phoneNumber;
        }

        public async Task<string> GetMessage(TransactionNotificationDTO transactionNotificationDTO,string smsType)
        {
            string randomNum = GenRefNo();
            PlainResponse response = new PlainResponse();
            string otp = randomNum.Substring(randomNum.Length - 6, 6) ?? "";
            string smsBody = string.Empty;
            //bool mailSent = true;
            if (TransactionTypes.Credit.ToString().ToLower() == smsType)
            {
                smsBody = PopulateCreditBodySMS(transactionNotificationDTO);
            }
            else if (TransactionTypes.Withdrawal.ToString().ToLower() == smsType)
            {
                smsBody = PopulateDebititBodySMS(transactionNotificationDTO);
            }
            else
            {
                smsBody = string.Empty;
            }

            return smsBody;
        }

        public static string PopulateCreditBodySMS(TransactionNotificationDTO transactionNotificationDTO)
        {
            string maskAcctNumber = MaskNumber(transactionNotificationDTO.AccountNumber);
            decimal balance = Convert.ToDecimal(transactionNotificationDTO.Balance);
            decimal amount = Convert.ToDecimal(transactionNotificationDTO.Amount);
            string body = string.Empty;
        
        // 2XX..00X
        //Amt:NGN 100,000.00
        //Des: AT129TRF2MPT1q9f51883043608954306560AT129TRF2M
        //Date:25 - 01 - 2025 07:46 //Jan. 25th, 2025 12:01:25 PM
        //Bal: NGN 144,982.70
            body = $"Txn: Credit\nAc: {maskAcctNumber}\nAmt:NGN {amount}\nDes:{transactionNotificationDTO.Description}\nDate: {transactionNotificationDTO.Date.ToString("dd-MM-yyyy HH:mm")}\nBal: NGN {balance}\nAscompay";
            return body;
        }
        public static string MaskNumber(string number)
        {
            if (number.Length != 10)
            {
                throw new ArgumentException("The input number must be exactly 10 digits long.");
            }

            return $"{number[0]}XX..00{number[^1]}";
        }

        public static string PopulateDebititBodySMS(TransactionNotificationDTO transactionNotificationDTO)
        {
            string maskAcctNumber = MaskNumber(transactionNotificationDTO.AccountNumber);
            decimal balance = Convert.ToDecimal(transactionNotificationDTO.Balance);
            decimal amount = Convert.ToDecimal(transactionNotificationDTO.Amount);
            string body = string.Empty;

            body = $"Txn: Debit\nAc: {maskAcctNumber}\nAmt:NGN {amount}\nDes:{transactionNotificationDTO.Description}\nDate: {transactionNotificationDTO.Date.ToString("dd-MM-yyyy HH:mm")}\nBal: NGN{balance}\nAscompay";
            return body;
        }


        private static string GenRefNo()
        {
            string result = string.Empty;
            result = DateTime.UtcNow.Ticks.ToString();
            return result;
        }
    }
}
