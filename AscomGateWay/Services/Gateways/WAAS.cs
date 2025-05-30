using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Helpers;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text;

namespace AscomPayPG.Services.Gateways
{
    public class WAAS
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;
        private readonly ITransactionHelper _transactionHelper;
        private readonly ILogger<WAAS> _logger;

        public WAAS(IConfiguration configuration,
                    AppDbContext context, 
                    IClientRequestRepository<ClientRequest> clientRequestRepo,
                    ITransactionHelper transactionHelper,
                    ILogger<WAAS>logger
                    )
        {
            _configuration = configuration;
            _context = context;
            _clientRequestRepo = clientRequestRepo;
            _transactionHelper = transactionHelper;
            _logger = logger;
        }


        public async Task<PlainResponse> GetAccessToken()
        {
            PlainResponse respObj = new PlainResponse();
            try
            {
                AuthenticateRequest requestData = new AuthenticateRequest();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                requestData.username = _configuration["WAASConfiguration:Username"];
                requestData.password = _configuration["WAASConfiguration:Password"];
                requestData.clientId = _configuration["WAASConfiguration:ClientId"];
                requestData.clientSecret = _configuration["WAASConfiguration:ClientSecret"];
                string fullUrl = string.Empty;


                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    fullUrl = $"{baseUrl}api/{version}/authenticate";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = "Ascompay",
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(requestData),
                            Response = apiResponse,
                            ResponseTime = DateTime.Now,
                            Service = "Payment",
                            Vendor = "9PSB",
                        };
                        _context.ExternalIntegrationLogs.Add(log);
                        _context.SaveChanges();
                        if (response.IsSuccessStatusCode)
                        {
                            respObj.IsSuccessful = true;
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Data = responseObj.accessToken;
                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.message;
                            respObj.Data = responseObj;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> OpenWallet(string userUid)
        {
            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            PlainResponse respWalletEnquiry = new PlainResponse();
            OpenWalletRequest requestData = new OpenWalletRequest();
            string bvn = string.Empty;
            BlueSaltBvnVerificationResponseDTO bvnVerificationResponse = new BlueSaltBvnVerificationResponseDTO();
            //bvnVerificationResponse.results = new Models.DTO.Results();
            //bvnVerificationResponse.results.personal_info = new PersonalInfo();
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                var appUser = await _context.Users.FirstOrDefaultAsync(x => x.UserUid == Guid.Parse(userUid));

                if (appUser != null)
                {
                    var UserExternalWalletEntity = await _context.UserExternalWallets.FirstOrDefaultAsync(x => x.UserUId == appUser.UserUid.ToString());
                    if (UserExternalWalletEntity == null)
                    {
                        var KycEntity = await _context.UserKycs.FirstOrDefaultAsync(x => x.UserUid == Guid.Parse(userUid) && string.IsNullOrEmpty(x.BlueSaltBVNVerificationResponse) == false && x.DocumentType.ToLower() == "bvn" && x.DocumentNumber != null);
                        if (KycEntity != null)
                        {
                            bvn = KycEntity.DocumentNumber;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            respObj.Message = "Please Complete Kyc Verification to continue";
                            respObj.Data = null;
                            return respObj;
                        }
                        if (!string.IsNullOrEmpty(bvn))
                        {
                            bvnVerificationResponse = JsonConvert.DeserializeObject<BlueSaltBvnVerificationResponseDTO>(KycEntity.BlueSaltBVNVerificationResponse);

                            if (bvnVerificationResponse == null)
                            {
                                requestData.accountName = $"{appUser.FirstName} {appUser.LastName}";
                                requestData.bvn = bvn;
                                requestData.dateOfBirth = appUser.DateOfBirth != null ? appUser.DateOfBirth.Value.ToString("dd/MM/yyyy").Replace("-", "/") : DateTime.Now.AddYears(-20).ToString("dd/MM/yyyy").Replace("-", "/");
                                requestData.gender = "1";
                                requestData.lastName = appUser.LastName;
                                requestData.otherNames = string.IsNullOrEmpty(appUser.MiddleName) ? "N/A" : appUser.MiddleName;
                                requestData.phoneNo = appUser.PhoneNumber;
                                requestData.transactionTrackingRef = userUid.Split("-").Last();  //DateTime.Now.Ticks.ToString(); //userUid.Split("-").Last();
                                requestData.placeOfBirth = "NA";
                                requestData.address = appUser.Address == null ? "NA" : appUser.Address;
                                requestData.nationalIdentityNo = "";
                                requestData.nextOfKinPhoneNo = "";
                                requestData.nextOfKinName = "";
                                requestData.email = appUser.Email;
                            }
                            else
                            {
                                requestData.accountName = $"{bvnVerificationResponse.results.personal_info.first_name} {bvnVerificationResponse.results.personal_info.last_name}";
                                requestData.bvn = bvn;
                                requestData.dateOfBirth = Convert.ToDateTime(bvnVerificationResponse.results.personal_info.date_of_birth).ToString("dd/MM/yyyy").Replace("-", "/"); // "01/01/2000";
                                requestData.gender = bvnVerificationResponse.results.personal_info.gender.ToLower().StartsWith("m") ? "0" : "1";
                                requestData.lastName = bvnVerificationResponse.results.personal_info.last_name;
                                requestData.otherNames = string.IsNullOrEmpty(bvnVerificationResponse.results.personal_info.middle_name) ? "N/A" : bvnVerificationResponse.results.personal_info.middle_name;
                                requestData.phoneNo = bvnVerificationResponse.results.personal_info.phone_number;
                                requestData.transactionTrackingRef = userUid.Split("-").Last();  //DateTime.Now.Ticks.ToString(); //userUid.Split("-").Last();
                                requestData.placeOfBirth = "NA";
                                requestData.address = appUser.Address == null ? "NA" : appUser.Address;
                                requestData.nationalIdentityNo = "";
                                requestData.nextOfKinPhoneNo = "";
                                requestData.nextOfKinName = "";
                                requestData.email = appUser.Email;
                            }

                        }
                        string fullUrl = string.Empty;
                        dynamic responseObj = new ExpandoObject();
                        using (var httpClient = new HttpClient())
                        {
                            httpClient.DefaultRequestHeaders.Accept.Clear();
                            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                            fullUrl = $"{baseUrl}api/{version}/open_wallet";
                            StringContent content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                            using (var response = await httpClient.PostAsync(fullUrl, content))
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();

                                var log = new ExternalIntegrationLog
                                {
                                    CreatedBy = userUid,
                                    RequestTime = DateTime.Now,
                                    RequestPayload = JsonConvert.SerializeObject(requestData),
                                    Response = apiResponse,
                                    ResponseTime = DateTime.Now,
                                    Service = "Payment",
                                    Vendor = "9PSB",
                                };
                                _context.ExternalIntegrationLogs.Add(log);
                                _context.SaveChanges();

                                if (response.IsSuccessStatusCode)
                                {

                                    responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                                    if ((int)response.StatusCode == StatusCodes.Status200OK)
                                    {
                                        if (responseObj.status == "SUCCESS")
                                        {
                                            respObj.IsSuccessful = true;
                                            respObj.Data = responseObj.data;
                                            //enquiry
                                            WalletRequest walletRequest = new WalletRequest()
                                            {
                                                accountNo = responseObj.data.accountNumber
                                            };

                                            respWalletEnquiry = await WalletEnquiry(walletRequest);
                                            if (respWalletEnquiry.IsSuccessful == true)
                                            {
                                                //save wallet details
                                                UserExternalWallet userExternalWallet = new UserExternalWallet();

                                                userExternalWallet.availableBalance = string.Empty;
                                                userExternalWallet.bvn = respWalletEnquiry.Data.bvn;
                                                userExternalWallet.email = appUser.Email;
                                                userExternalWallet.firstName = bvnVerificationResponse == null ? appUser.FirstName : bvnVerificationResponse.results.personal_info.first_name;
                                                userExternalWallet.lastName = bvnVerificationResponse == null ? appUser.LastName : bvnVerificationResponse.results.personal_info.last_name;
                                                userExternalWallet.phoneNo = appUser.PhoneNumber;
                                                userExternalWallet.freezeStatus = respWalletEnquiry.Data.freezeStatus;
                                                userExternalWallet.ledgerBalance = string.Empty;
                                                userExternalWallet.lienStatus = respWalletEnquiry.Data.lienStatus;
                                                userExternalWallet.maximumBalance = string.Empty;
                                                userExternalWallet.maximumDeposit = string.Empty;
                                                userExternalWallet.name = respWalletEnquiry.Data.name;
                                                userExternalWallet.nuban = respWalletEnquiry.Data.nuban;
                                                userExternalWallet.number = respWalletEnquiry.Data.number;
                                                userExternalWallet.pndstatus = respWalletEnquiry.Data.pndstatus;
                                                userExternalWallet.status = respWalletEnquiry.Data.status;
                                                userExternalWallet.productCode = respWalletEnquiry.Data.productCode;
                                                userExternalWallet.tier = respWalletEnquiry.Data.tier;
                                                userExternalWallet.UserUId = appUser.UserUid.ToString();
                                                userExternalWallet.totalWalletBalance = string.Empty;
                                                userExternalWallet.IsActive = true;
                                                userExternalWallet.IsDeprecated = false;

                                                _context.Add(userExternalWallet);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            respObj.IsSuccessful = false;
                                            respObj.Data = null;
                                            respObj.Message = responseObj.message;
                                        }

                                    }
                                    else
                                    {
                                        respObj.IsSuccessful = false;
                                        respObj.Data = null;
                                    }

                                    respObj.ResponseCode = (int)response.StatusCode;
                                    respObj.Message = responseObj.message;
                                }
                                else
                                {
                                    respObj.IsSuccessful = false;
                                    apiResponse = await response.Content.ReadAsStringAsync();
                                    responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                                    respObj.Message = responseObj.message;
                                    respObj.Data = null;
                                    respObj.ResponseCode = (int)response.StatusCode;
                                }
                            }
                        }
                    }
                    else
                    {
                        respObj.IsSuccessful = true;
                        respObj.Message = "External Wallet Already Exist for user";
                        respObj.Data = null;
                        return respObj;
                    }

                }
                else
                {
                    respObj.IsSuccessful = false;
                    respObj.Message = "Invalid user";
                    respObj.Data = null;
                    return respObj;
                }

            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> WalletEnquiry(WalletRequest model)
        {

            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;


                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/wallet_enquiry";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = "Ascompay",
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(model),
                            Response = apiResponse,
                            ResponseTime = DateTime.Now,
                            Service = "Payment",
                            Vendor = "9PSB",
                        };
                        _context.ExternalIntegrationLogs.Add(log);
                        _context.SaveChanges();
                        if (response.IsSuccessStatusCode)
                        {
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                                //update account balance
                                //var accountEntity = await _clientRequestRepo.GetUserAccount(model.accountNo);
                                //if (accountEntity != null)
                                //{
                                //    decimal amount = Convert.ToDecimal(responseObj.data.availableBalance);
                                //    decimal newBalance = await UpdateDestinationAccountBalance(accountEntity, amount);
                                //}
                                //else
                                //{
                                //    respObj.IsSuccessful = false;
                                //    respObj.Data = null;
                                //}

                                respObj.ResponseCode = (int)response.StatusCode;
                                respObj.Message = responseObj.message;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                apiResponse = await response.Content.ReadAsStringAsync();
                                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                                respObj.Message = responseObj.message;
                                respObj.Data = null;
                                respObj.ResponseCode = (int)response.StatusCode;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> UpgradeToTier3(WalletUpgradeTier3Request model)
        {

            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;


                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/walletUpgrade-tier3-base64";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var requestDataTemperedTolOg = model;
                        requestDataTemperedTolOg.proofOfAddressVerification = "";
                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = "Ascompay",
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(requestDataTemperedTolOg),
                            Response = apiResponse,
                            ResponseTime = DateTime.Now,
                            Service = "Verification",
                            Vendor = "9PSB",
                        };
                        _context.ExternalIntegrationLogs.Add(log);
                        _context.SaveChanges();
                        if (response.IsSuccessStatusCode)
                        {
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                                respObj.ResponseCode = (int)response.StatusCode;
                                respObj.Message = responseObj.message;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                apiResponse = await response.Content.ReadAsStringAsync();
                                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                                respObj.Message = responseObj.message;
                                respObj.Data = null;
                                respObj.ResponseCode = (int)response.StatusCode;
                            }
                        }
                        else
                        {
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.IsSuccessful = false;
                            respObj.Data = null;
                            respObj.Message = responseObj.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> UpgradeToTier2(WalletUpgradeTier2Request model)
        {

            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;


                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/wallet_upgrade";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var requestDataTemperedTolOg = model;

                        requestDataTemperedTolOg.proofOfAddressVerification = "";
                        requestDataTemperedTolOg.customerSignature = "";
                        requestDataTemperedTolOg.userPhoto = "";
                        requestDataTemperedTolOg.idCardFront = "";
                        requestDataTemperedTolOg.idCardBack = "";
                        requestDataTemperedTolOg.utilityBill = "";

                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = "Ascompay",
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(requestDataTemperedTolOg),
                            Response = apiResponse,
                            ResponseTime = DateTime.Now,
                            Service = "Verification",
                            Vendor = "9PSB",
                        };
                        _context.ExternalIntegrationLogs.Add(log);
                        _context.SaveChanges();
                        if (response.IsSuccessStatusCode)
                        {
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                                respObj.ResponseCode = (int)response.StatusCode;
                                respObj.Message = responseObj.message;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                apiResponse = await response.Content.ReadAsStringAsync();
                                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                                respObj.Message = responseObj.message;
                                respObj.Data = null;
                                respObj.ResponseCode = (int)response.StatusCode;
                            }
                        }
                        else
                        {
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.IsSuccessful = false;
                            respObj.Data = null;
                            respObj.Message = responseObj.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }

        public async Task<PlainResponse> WalletStatus(WalletRequest model)
        {
            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;


                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/wallet_status";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = model.accountNo,
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(model),
                            Response = apiResponse,
                            ResponseTime = DateTime.Now,
                            Service = "Payment",
                            Vendor = "9PSB",
                        };
                        _context.ExternalIntegrationLogs.Add(log);
                        _context.SaveChanges();
                        if (response.IsSuccessStatusCode)
                        {

                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                respObj.Data = null;
                            }

                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.message;
                            respObj.Data = null;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> GetWallet(WalletRequest model)
        {
            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;
                var walletEntity = await _context.UserExternalWallets.FirstOrDefaultAsync(x => x.nuban == model.accountNo);
                if (walletEntity == null)
                {
                    respObj.IsSuccessful = false;
                    respObj.Message = "No records found for wallet";
                    respObj.Data = null;
                    return respObj;
                }
                GetWalletRequest getWalletRequest = new GetWalletRequest()
                {
                    bvn = walletEntity.bvn
                };

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/get_wallet";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(getWalletRequest), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = model.accountNo,
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(model),
                            Response = apiResponse,
                            ResponseTime = DateTime.Now,
                            Service = "Payment",
                            Vendor = "9PSB",
                        };
                        _context.ExternalIntegrationLogs.Add(log);
                        _context.SaveChanges();
                        if (response.IsSuccessStatusCode)
                        {

                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                respObj.Data = null;
                            }

                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.message;
                            respObj.Data = null;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> ChangeWalletStatus(ChangeWalletStatusRequest model)
        {
            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/change_wallet_status";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {

                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                respObj.Data = null;
                            }

                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.message;
                            respObj.Data = null;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> WalletTransactions(WalletTransactionRequest model)
        {
            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/wallet_transactions";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            var log = new ExternalIntegrationLog
                            {
                                CreatedBy = model.accountNumber,
                                RequestTime = DateTime.Now,
                                RequestPayload = JsonConvert.SerializeObject(model),
                                Response = apiResponse,
                                ResponseTime = DateTime.Now,
                                Service = "Payment",
                                Vendor = "9PSB",
                            };
                            _context.ExternalIntegrationLogs.Add(log);
                            _context.SaveChanges();

                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                respObj.Data = null;
                            }

                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.message;
                            respObj.Data = null;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> WalletRequery(WalletRequeryRequest model)
        {
            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/wallet_requery";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = model.accountNo,
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(model),
                            Response = apiResponse,
                            ResponseTime = DateTime.Now,
                            Service = "Payment",
                            Vendor = "9PSB",
                        };
                        _context.ExternalIntegrationLogs.Add(log);
                        _context.SaveChanges();
                        if (response.IsSuccessStatusCode)
                        {

                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                respObj.Data = null;
                            }

                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.message;
                            respObj.Data = null;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> WalletDebit(DebitWalletRequest model)
        {
            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/debit/transfer";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        var log = new ExternalIntegrationLog
                        {
                            CreatedBy = model.accountNo,
                            RequestTime = DateTime.Now,
                            RequestPayload = JsonConvert.SerializeObject(model),
                            Response = apiResponse,
                            ResponseTime = DateTime.Now,
                            Service = "Payment",
                            Vendor = "9PSB",
                        };
                        _context.ExternalIntegrationLogs.Add(log);
                        _context.SaveChanges();
                        if (response.IsSuccessStatusCode)
                        {

                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                respObj.Data = null;
                            }

                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.message;
                            respObj.Data = null;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
        public async Task<PlainResponse> WalletCredit(CreditWalletRequest model)
        {
            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/credit/transfer";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {

                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if ((int)response.StatusCode == StatusCodes.Status200OK)
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                                

                                WalletRequest recipientWalletRequest = new WalletRequest()
                                {
                                    accountNo = model.accountNo
                                };
                                PlainResponse getRecipientWallet = await WalletEnquiry(recipientWalletRequest);
                                var accountEntity = await _clientRequestRepo.GetUserAccount(model.accountNo);
                                if(accountEntity != null)
                                {
                                    var receiver = await _context.Users.FirstOrDefaultAsync(a => a.UserUid == accountEntity.UserUid);
                                 //   await _transactionHelper.NotifyForCredit($"{receiver.FirstName} {receiver.LastName}", receiver.Email,
                                 //$"{model..FirstName} {sender.LastName}",
                                 //model.totalAmount.ToString(),
                                 //getRecipientWallet.Data.ledgerBalance.ToString(),
                                 //DateTime.Now.ToString(), model.narration);
                                    //send sms notification
                                    await _transactionHelper.NotifyForCreditSMS(receiver, model.accountNo, model.totalAmount.ToString(), getRecipientWallet.Data.ledgerBalance.ToString(), model.narration);
                                }
                                
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                respObj.Data = null;
                            }

                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.message;
                            respObj.Data = null;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }

        public async Task<PlainResponse> TransferOtherBank(OtherBankTransferDTO model,
                                                          string userId,
                                                          string sendAccountName,
                                                           string lookUpNumber,
                                                           string lookUpName,
                                                           bool isInternal = false, 
                                                           bool isAccountToAccount = false, 
                                                           string transactionRef = "")
        {

            string transactionType = isInternal ? TransactionTypes.TransferToAscomUsers.ToString() : TransactionTypes.TransferToOthersBanks.ToString();

            PlainResponse respObj = new PlainResponse();
            PlainResponse respAccessToken = new PlainResponse();
            string bvn = string.Empty;
            var sourceAccount = await _context.Accounts.Include(x => x.AccountTeir).FirstOrDefaultAsync(x => x.AccountNumber == model.senderAccountNumber);
            var sender = _context.Users.FirstOrDefault(x => x.UserUid.ToString() == userId);
            var paymentProviderCharges = await GetPaymentProviderCharges(transactionType);
            var marchantCharge = await CalculateCharges(decimal.Parse(model.Amount), transactionType);
            var charges = paymentProviderCharges + marchantCharge;
            var vat = await _transactionHelper.CalculateVAT(decimal.Parse(model.Amount) + charges, transactionType);
            string senderNewBalance = string.Empty;
          
            if (sender == null)
            {
                return new PlainResponse
                {
                    IsSuccessful = false,
                    Message = "user account account does exist",
                    Data = 0,
                };
            }

            if (sourceAccount == null)
            {
                return new PlainResponse
                {
                    IsSuccessful = false,
                    Message = "source account account does exist",
                    Data = 0,
                };
            }

            if ((decimal)sourceAccount.CurrentBalance < decimal.Parse(model.Amount) + charges + vat)
            {
                return new PlainResponse
                {
                    IsSuccessful = false,
                    Message = "insufficient balance",
                    Data = 0,
                };
            }

            var transactionReference =  string.IsNullOrEmpty(transactionRef) ? Guid.NewGuid().ToString().Substring(0, 20).Replace("-", "").ToUpper() : transactionRef;
            var regResponse = await _clientRequestRepo.RegisterTransaction(decimal.Parse(model.Amount), paymentProviderCharges, marchantCharge,
                                                                                  lookUpName, sender, transactionReference, decimal.Parse(model.Amount) + charges,
                                                                                  model.Description, transactionType, vat, charges, PaymentProvider.NinePSB.ToString(),
                                                                                  model.senderAccountNumber, lookUpNumber, "", "");
            if (!regResponse)
            {
                return new PlainResponse
                {
                    IsSuccessful = false,
                    Message = "Something went wrong while processing request",
                    Data = 0,
                };
            }

            try
            {
                respAccessToken = await GetAccessToken();
                string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
                string version = _configuration["WAASConfiguration:Version"];
                string fullUrl = string.Empty;

                var payload = new OtherBankTransfer();
                var externalUsers = _context.UserExternalWallets.FirstOrDefault(user => user.nuban == model.senderAccountNumber);

                payload.customer.account = new Models.WAAS.Account
                {
                    bank = isInternal ? "120001" : model.bank,
                    senderaccountnumber = model.senderAccountNumber,
                    number = lookUpNumber,
                    name =   lookUpName,
                    sendername = externalUsers != null ? externalUsers.name : "N/A",
                };
                payload.order.country = "NGN";
                payload.order.currency = "Nigeria";
                payload.transaction.sessionId = "";
                payload.transaction.reference = transactionReference;
                payload.merchant.merchantFeeAmount = marchantCharge.ToString(); // getMarchant
                payload.merchant.merchantFeeAccount = "";
                payload.narration = model.Narration;
                payload.order.description = model.Description;
                decimal amountToSend = decimal.Parse(model.Amount) + marchantCharge;
                payload.order.amount = amountToSend.ToString();
                payload.message = "";
                payload.code = "";

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                    fullUrl = $"{baseUrl}api/{version}/wallet_other_banks";

                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new CustomContractResolver(),
                        Formatting = Formatting.Indented
                    };

                    var payloadLoadAsJsonString = JsonConvert.SerializeObject(payload);

                    ExternalIntegrationLog externalIntegrationLog = new ExternalIntegrationLog
                    {
                        CreatedBy = userId,
                        DateCreated = DateTime.Now,
                        RequestTime = DateTime.Now,
                        RequestPayload = payloadLoadAsJsonString,
                        Vendor = "9PSB",
                        Service = "Wallet/TransferToOtherBanks"
                    };

                    StringContent content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        externalIntegrationLog.Response = await response.Content.ReadAsStringAsync();
                        externalIntegrationLog.ResponseTime = DateTime.Now;
                        _context.ExternalIntegrationLogs.Add(externalIntegrationLog);
                        _context.SaveChanges();

                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();

                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            //(responseObj.data.code == ""
                            if (responseObj?.data.code == "00")
                            {
                                respObj.IsSuccessful = true;
                                respObj.Data = responseObj.data;
                                respObj.transaction_reference = payload.transaction.reference;
                                var updateResponse = await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Successful.ToString());

                                respObj.ResponseCode = (int)response.StatusCode;
                                respObj.Message = responseObj.data.message;

                                WalletRequest senderWalletRequest = new WalletRequest()
                                {
                                    accountNo = sourceAccount.AccountNumber
                                };

                                PlainResponse getSenderWallet = await WalletEnquiry(senderWalletRequest);

                                if (isInternal)
                                {

                                    var receiverAccount = await _clientRequestRepo.GetAccount(lookUpNumber);
                                    var receiver = await _clientRequestRepo.GetUser(receiverAccount.UserUid.ToString());

                                    PlainResponse receiverWallet = await WalletEnquiry(new WalletRequest { accountNo = lookUpNumber });
                                    if (receiverWallet != null)
                                    {
                                        _transactionHelper.NotifyForCredit($"{receiver.FirstName} {receiver.LastName}", receiver.Email,
                                                                           $"{sourceAccount.AccountName}", amountToSend.ToString(),
                                                                            receiverWallet.Data.ledgerBalance.ToString(),
                                                                            DateTime.Now.ToString(), payload.narration);


                                        await _transactionHelper.NotifyForCreditSMS(receiver,
                                                                          receiverAccount.AccountNumber,
                                                                          amountToSend.ToString().ToString(),
                                                                          receiverWallet.Data.ledgerBalance.ToString(),
                                                                          model.Description);
                                    }
                                }

                                _transactionHelper.NotifyForDebit(sender.Email, $"{sender.FirstName} {sender.LastName}",
                                                                  amountToSend.ToString(), getSenderWallet.Data.ledgerBalance.ToString(),
                                                                  vat.ToString(), charges.ToString(), DateTime.Now.ToString(),
                                                                  payload.narration,
                                                                  transactionReference);

                                await _transactionHelper.NotifyForDebitSMS(sender,
                                                                           sourceAccount.AccountNumber,
                                                                           amountToSend.ToString().ToString(),
                                                                           getSenderWallet.Data.ledgerBalance.ToString(),
                                                                           model.Description);
                            }
                            else
                            {
                                respObj.IsSuccessful = false;
                                respObj.Data = null;
                                respObj.Message = responseObj.data.message;
                                var updateResponse = await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Failed.ToString());
                            }
                        }
                        else
                        {
                           var updateResponse = await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Failed.ToString());
                            
                            respObj.IsSuccessful = false;
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Message = responseObj.data.message;
                            respObj.Data = null;
                            respObj.ResponseCode = (int)response.StatusCode;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR:::::::::::::::::::::::::::::::::::::::::{ex.Message} {ex.StackTrace}");

                var updateResponse = await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Failed.ToString());
                respObj.IsSuccessful = false;
                respObj.Message = ex.Message;
                respObj.Data = null;
                return respObj;
            }
            return respObj;
        }
       
        public async Task<decimal> UpdateDestinationAccountBalance(Models.DTO.Account account, decimal amount)
        {
            var currentBalance = account.CurrentBalance;
            account.CurrentBalance += amount;
            account.PrevioseBalance = currentBalance;
            var walletsBalance = await GetUserTotalWalletBalance(account.UserUid.ToString());
            account.LegerBalance = account.CurrentBalance + walletsBalance;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account.CurrentBalance ?? 0;
        }
        private async Task<decimal> GetUserTotalWalletBalance(string userId)
        {
            var userWallets = _context.UserWallets.Where(x => x.UserUid.ToString() == userId).ToList();
            if (userWallets.Count() > 1)
            {
                return userWallets.Sum(uw => uw.CurrentBalance) ?? 0;
            }
            return 0;
        }
        public async Task<decimal> GetPaymentProviderCharges(string transactionType)
        {
            var charges = await _context.TransactionType.FirstOrDefaultAsync(x => x.Ttype.Replace(" ", "").Replace("(", "").Replace(")", "") == transactionType);
            return charges == null ? 0 : charges.T_Provider_Charges;
        }
        public async Task<decimal> CalculateCharges(decimal amount, string transactionType)
        {
            var transactionTypeDetails = _context.TransactionType.FirstOrDefault(x => x.Ttype
                                                                                      .Replace(" ", "")
                                                                                      .Replace("(", "")
                                                                                      .Replace(")", "") == transactionType);
            if (transactionTypeDetails != null)
            {
                if (transactionTypeDetails.By_Percent)
                    return Math.Round(transactionTypeDetails.T_Percentage / 100 * amount, 1);
                else
                    return transactionTypeDetails.T_Amount;
            }
            return 0;
        }
        public async Task<AccountLookUpResponse> AccountLookup9PSB(accountLookupRequest accountLookupRequest, string userId)
        {
            //PlainResponse respObj = new PlainResponse();

            // check user
            var sender = _context.Users.FirstOrDefault(x => x.UserUid.ToString() == userId);
            if (sender == null)
            {
                return new AccountLookUpResponse
                {
                    IsSuccessful = false,
                    Message = "user does exist",
                    Data = null,
                };
            }
            // check user account
            var sourceAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.UserUid.ToString() == userId);
            if (sourceAccount == null)
            {
                return new AccountLookUpResponse
                {
                    IsSuccessful = false,
                    Message = "user account account does exist",
                    Data = null,
                };
            }


            var requestPayLoad = new Root9PSBAccVerificationPayLoad();
            requestPayLoad.customer.account.senderaccountnumber = sourceAccount.AccountNumber;
            requestPayLoad.customer.account.number = accountLookupRequest.account_number;
            requestPayLoad.customer.account.bank = accountLookupRequest.bank_code;

            var respAccessToken = await GetAccessToken();
            string baseUrl = _configuration["WAASConfiguration:BaseUrl"];
            string version = _configuration["WAASConfiguration:Version"];
            string fullUrl = string.Empty;

            dynamic responseObj = new ExpandoObject();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {respAccessToken.Data}");
                fullUrl = $"{baseUrl}api/{version}/other_banks_enquiry";

                var payloadLoadAsJsonString = JsonConvert.SerializeObject(requestPayLoad);

                ExternalIntegrationLog externalIntegrationLog = new ExternalIntegrationLog
                {
                    CreatedBy = userId,
                    DateCreated = DateTime.Now,
                    RequestTime = DateTime.Now,
                    RequestPayload = payloadLoadAsJsonString,
                    Vendor = "9PSB",
                    Service = "Wallet/other_banks_enquiry"
                };

                StringContent content = new StringContent(payloadLoadAsJsonString, Encoding.UTF8, "application/json");
                AccountLookUpResponse accountLookUpResponse = new AccountLookUpResponse();



                try
                {
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {

                        externalIntegrationLog.Response = await response.Content.ReadAsStringAsync();
                        externalIntegrationLog.ResponseTime = DateTime.Now;
                        _context.ExternalIntegrationLogs.Add(externalIntegrationLog);
                        _context.SaveChanges();

                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();

                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            if (responseObj?.code == "00")
                            {
                                accountLookUpResponse.IsSuccessful = true;
                                accountLookUpResponse.ResponseCode = StatusCodes.Status200OK;
                                accountLookUpResponse.Message = responseObj.message;
                                accountLookUpResponse.Data.account_number = responseObj.customer.account.number;
                                accountLookUpResponse.Data.account_name = responseObj.customer.account.name;

                                // save response 
                                var lookUp = new AccountLookUpLog
                                {
                                    AccountName = responseObj.customer.account.name,
                                    AccountNumber = responseObj.customer.account.number,
                                    UsageStatus = (int)AccountLookUpUsageStatus.Init,
                                    LookUpBank = accountLookupRequest.bank_code,
                                    LookStatus = true,
                                    InitaitorId = userId,
                                    LookUpId = Guid.NewGuid().ToString().Substring(0, 18).Replace("-", ""),
                                };
                                _context.AccountLookUpLog.Add(lookUp);
                                _context.SaveChanges();
                                accountLookUpResponse.LRId = lookUp.LookUpId;
                                return accountLookUpResponse;
                            }
                            else
                            {

                                // save response 
                                var lookUp = new AccountLookUpLog
                                {
                                    AccountName =string.Empty,
                                    AccountNumber = accountLookupRequest.account_number,
                                    UsageStatus = (int)AccountLookUpUsageStatus.failed,
                                    LookUpBank = accountLookupRequest.bank_code,
                                    InitaitorId = userId,
                                    LookUpId = string.Empty,
                                };
                                _context.AccountLookUpLog.Add(lookUp);
                                _context.SaveChanges();
                                accountLookUpResponse.LRId = lookUp.LookUpId;

                                accountLookUpResponse.IsSuccessful = false;
                                accountLookUpResponse.Data = null;
                                accountLookUpResponse.ResponseCode = StatusCodes.Status400BadRequest;
                                accountLookUpResponse.Message = responseObj.message;
                                return accountLookUpResponse;
                            }
                        }
                        else
                        {

                            // save response 
                            var lookUp = new AccountLookUpLog
                            {
                                AccountName = string.Empty,
                                AccountNumber = accountLookupRequest.account_number,
                                UsageStatus = (int)AccountLookUpUsageStatus.failed,
                                LookUpBank = accountLookupRequest.bank_code,
                                InitaitorId = userId,
                                LookUpId = Guid.NewGuid().ToString().Substring(0, 18).Replace("-", ""),
                            };
                            _context.AccountLookUpLog.Add(lookUp);
                            _context.SaveChanges();
                            accountLookUpResponse.LRId = lookUp.LookUpId;

                            accountLookUpResponse.IsSuccessful = false;
                            accountLookUpResponse.Data = null;
                            accountLookUpResponse.ResponseCode = StatusCodes.Status400BadRequest;
                            accountLookUpResponse.Message = responseObj.message;
                            return accountLookUpResponse;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

    }
}