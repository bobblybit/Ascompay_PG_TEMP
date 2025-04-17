using System.Net;
using Polly;
using System.Text;
using AscomPayPG.Models;
using AscomPayPG.Models.Shared;
using Newtonsoft.Json;
using RestSharp;
using AscomPayPG.Data;
using AscomPayPG.Models.DTO;
using AscomPayPG.Helpers.HTTPHelper;
using System;
using Microsoft.EntityFrameworkCore;
using AscomPayPG.Data.Enum;
using AscomPayPG.Helpers;

namespace AscomPayPG.Services
{
    public class HelperService : IHelperService
    {
        private readonly ILogger<HelperService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITransactionsRepository<Transactions> _transHistoryRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public HelperService(
            AppDbContext context,
            ILogger<HelperService> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ITransactionsRepository<Transactions> transHistoryRepository
        )
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _transHistoryRepo = transHistoryRepository;
        }

        public Task CustomLogError(Exception e, string action)
        {
            string message = e.Message;
            if (e.InnerException != null && e.InnerException.Message != null)
            {
                message = e.InnerException.Message;
            }
            _logger.LogError($"{action}:: {message}!");
            return Task.CompletedTask;
        }

        public Task<string> GetConfigItem(string configName)
        {
            string result = _configuration[$"{configName}"];
            return Task.FromResult(result);
        }


        public async Task<ResponseMessage> SessionValidation(string accessToken)
        {
            ResponseMessage Apiresponse = new ResponseMessage();
            try
            {
                string url = _configuration["App:AuthenticationServiceApi"];
                Dictionary<string, string> headerOptions = new Dictionary<string, string>()
                {
                    { "Content-Type", "application/json"},
                    { "accessToken" , accessToken }
                };

                url += "api/Security/verifySession";

                var postResponse = await GetResource(url, headerOptions);
                if (!postResponse.Status) throw new Exception($"User Auth request failed! {postResponse.Message}");
                _logger.LogInformation(JsonConvert.SerializeObject(postResponse.Data.First()));
                ResponseMessage? responseModel = JsonConvert.DeserializeObject<ResponseMessage>(postResponse.Data.First());
                if (responseModel == null) throw new Exception($"User Auth deserilization failed. {responseModel}");
                Apiresponse = responseModel;
            }
            catch (System.Exception ex)
            {
                await CustomLogError(ex, "UserValidation");
            }

            return Apiresponse!;
        }

        public Transactions GetPaymentDefaultModel(PaymentRequest payReq, string reference, string description)
        {
            var checkTransaction = GetOneTransactionType(payReq.TransactionType);
            if (checkTransaction == null) throw new Exception($"TransactionType not defined for this operation");

            var newTransactions = new Transactions();
            var checkUser = _context.Users.Where(a => a.UserUid == Guid.Parse(payReq.Uid) && !string.IsNullOrEmpty(a.Email)).FirstOrDefault();
            if (checkUser != null)
            {

                newTransactions.PaymentAction = PaymentActionType.Payment.ToString();
                newTransactions.UserId = checkUser.UserId;
                newTransactions.RequestTransactionId = reference;
                newTransactions.UserUID = Guid.Parse(payReq.Uid);
                newTransactions.Status = PaymentStatus.Init.ToString();
                newTransactions.StatusId = 1;
                newTransactions.Amount = payReq.TransactionsAmount;
                newTransactions.Email = !string.IsNullOrEmpty(checkUser.Email) ? checkUser.Email : string.Empty;
                newTransactions.CallbackURL = payReq.CallbackURL;
                newTransactions.Description = description;
                newTransactions.TransactionType = checkTransaction.Ttype;

                if (payReq.TransactionType == 1)
                {
                    newTransactions.DestinationAccount = payReq.destination;
                }
                else if (payReq.TransactionType == 2)
                {
                    newTransactions.DestinationWallet = payReq.destination;
                }
                else
                {
                    throw new Exception($"TransactionType not defined for this operation");
                }

                newTransactions.AccessToken = GetToken();
            }

            return newTransactions;
        }

        public string GetToken()
        {
            var session = _httpContextAccessor?.HttpContext?.Session?.GetString("token");

            if (string.IsNullOrEmpty(session))
            {
                throw new Exception($"InValid Session, Kindly request for Authorization.");
            }

            return session;
        }

        public async Task<AppResult<string>> GetResource(string url, Dictionary<string, string> headers, Dictionary<string, string> parameters = null)
        {
            AppResult<string> result = new AppResult<string>();
            try
            {
                if (string.IsNullOrEmpty(url)) throw new Exception($"url to query cannot be null or empty");
                RestClient client = new RestClient(url);
                RestRequest apiRequest = new RestRequest(string.Empty, Method.Get);

                if (headers.Any())
                {
                    foreach (var item in headers)
                    {
                        apiRequest.AddHeader(item.Key, item.Value);
                    }
                }
                if (parameters != null && parameters.Any())
                {
                    foreach (var item in parameters)
                    {
                        apiRequest.AddParameter(item.Key, item.Value);
                    }
                }

                var policy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
                RestResponse response = await policy.ExecuteAsync(async () => await client.GetAsync(apiRequest));
                _logger.LogInformation($"Get Response: \n{response.Content}");

                if (response == null) throw new Exception($"{url} response is null");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result.Status = true;
                    result.Message = "success";
                    result.Data.Add(response.Content!);
                }
                else
                {
                    string errorMessage = (!string.IsNullOrEmpty(response.ErrorException.Message)) ? response.ErrorException.Message : response.StatusDescription;
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {
                await CustomLogError(ex, "GetResource");
                result.Status = false;
                result.Message = ex.Message;
                result.Data = null;
            }
            return result;
        }

        public async Task<AppResult<string>> PostResource(string url, Dictionary<string, string> headers, object data, Dictionary<string, string> parameters = null)
        {
            AppResult<string> result = new AppResult<string>();
            try
            {
                if (string.IsNullOrEmpty(url)) throw new Exception($"url to query cannot be null or empty");
                RestClient client = new RestClient(url);
                RestRequest apiRequest = new RestRequest(string.Empty, Method.Post);
                if (headers.Any())
                {
                    foreach (var item in headers)
                    {
                        apiRequest.AddHeader(item.Key, item.Value);
                    }
                }
                if (parameters != null && parameters.Any())
                {
                    foreach (var item in parameters)
                    {
                        apiRequest.AddParameter(item.Key, item.Value);
                    }
                }

                apiRequest.AddJsonBody(data);
                var policy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
                RestResponse response = await policy.ExecuteAsync(async () => await client.PostAsync(apiRequest));
                _logger.LogInformation($"Post Response: \n{response.Content}");
                if (response == null) throw new Exception($"{url} response is null");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result.Status = true;
                    result.Message = "success";
                    result.Data.Add(response.Content!);
                }
                else
                {
                    string errorMessage = (!string.IsNullOrEmpty(response.ErrorException.Message)) ? response.ErrorException.Message : response.StatusDescription;
                    throw new Exception(errorMessage);
                }

            }
            catch (Exception ex)
            {
                await CustomLogError(ex, "PostResource");
                result.Status = false;
                result.Message = ex.Message;
                result.Data = null;
            }
            return result;
        }


        public async Task<string?> GetNewReferenceId()
        {
            string? result = null;
            try
            {
                var utcTimeStamp = GetUnixTimeStamp();
                result = string.Format("ASCOm{0}{1}", "Pt", utcTimeStamp);
                //result = GenerateTransactionReference();
                bool isAvailable = await _transHistoryRepo.IsRequestTransactionIdAvailable(result);
                while (!isAvailable)
                {
                    utcTimeStamp = GetUnixTimeStamp();
                    //result = GenerateTransactionReference();
                    result = string.Format("ASCOm{0}{1}", "Pt", utcTimeStamp);
                    isAvailable = await _transHistoryRepo.IsRequestTransactionIdAvailable(result);
                }

            }
            catch (System.Exception ex)
            {
                await CustomLogError(ex, "GetNewReferenceId");
            }
            return result;
        }

        private string GetUnixTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var result = String.Format("{0:0}", ts.TotalMilliseconds);
            return result;
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public TransactionType GetOneTransactionType(int itemId)
        {
            try
            {
                var item = _context.TransactionType.Where(x => x.TiD == itemId).FirstOrDefault();
                return item;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }
        public  Task<User> GetUserBySessionAsync(string session)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.CurrentSession == session);
        }
        public async Task<bool> ValidateTransaction(string accessToken, string senderAccount, string receiverAccount, decimal amount, string transactionType)
        {
            string url = _configuration["App:TransactionAuthentication"];
            if (new[] { senderAccount, receiverAccount, accessToken }.Any(string.IsNullOrWhiteSpace))
                return false;

            var requestModel = new TransactionVerificationRequestDTO
            {
                Amount = amount.ToString(),
                ReceiverAccount= receiverAccount,
                SourceAccount= senderAccount,
                Token = accessToken,
                TransactionType= transactionType
            };

            var header = new Dictionary<string, string> { { "Authorization", $"Bearer {accessToken}" } };
            StringContent content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");
            var response = await RequestHelper.PostWithBody(url, content);
            string apiResponse = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<ApiBaseResponse<TransactionVerificationResponseDTO>>(apiResponse);
            
            if (responseObj.Data.IsOk)
                return true;
            return false;
        }

        public Task<AccountLookUpLog> GetLookUpLog(string lookUpId)
        {

            string userId = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<string>("UserUid");


            return _context.AccountLookUpLog
                                     .OrderByDescending(x => x.DateCreated)
                                     .FirstOrDefaultAsync(x => x.InitaitorId == userId
                                                      && lookUpId == x.LookUpId
                                                      && x.LookStatus == true
                                                      && x.UsageStatus == (int)AccountLookUpUsageStatus.Init
                                                      );
        }

        public Task<UserSession> GetUserCurrentSessionAsync(string sessionToken, string refreshToken)
        {
            return _context.UserSession.FirstOrDefaultAsync(x => x.Token == sessionToken && x.RereshToken == refreshToken && x.IsActive == true);
        }
    }
}