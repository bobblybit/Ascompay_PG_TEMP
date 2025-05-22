using AscomPayPG.Data;
using AscomPayPG.Helpers;
using AscomPayPG.Helpers.HTTPHelper;
using AscomPayPG.Helpers.HTTPHelper.UrlHelper;
using AscomPayPG.Models;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.VasModels;
using AscomPayPG.Services.Gateways.Interface;
using AscomPayPG.Services.Interface;
using Nancy;
using Newtonsoft.Json;
using PaymentGateWayMiddleWare.Model;
using System;
using System.Dynamic;
using System.Text;

namespace AscomPayPG.Services.Gateways.Implementation
{
    public class Vas9PSB : I9psbVaS
    {
        private readonly AppDbContext _appdbContext;
        private readonly ITransactionHelper _transactionHelper;

        public Vas9PSB(AppDbContext appdbContext, ITransactionHelper transactionHelper) 
        {
            _appdbContext = appdbContext;
            _transactionHelper = transactionHelper;
        }    
        private async Task<VatAuthResponse> Authenticate()
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var userName = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVasConfiuration:Username");
                var password = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVasConfiuration:Password");

                var payload = new AuthenticationRequestModeL { username = userName, password = password };
                StringContent content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.PGM_Authenticate, content);

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                    if (responseObj?.responseCode == "200")
                        return new VatAuthResponse { IsSuccessfull = true, Token = responseObj.data.accessToken };
                    else
                        return new VatAuthResponse { IsSuccessfull = false, Token = responseObj.message };
            }
            catch (Exception ex)
            {
                return new VatAuthResponse { IsSuccessfull = false, Token = ex.Message };
            }
        }

        #region TOP-UP
        public async Task<Nine9psbGenResponse<PhoneNumberLookUpResponse>> GetPhoneNetwork(string phoneNumber)
        {
            try
            {
                var responseObj = new Nine9psbGenResponse<PhoneNumberLookUpResponse>();
            
                var url = NinePSBVatUrls.PGM_PhoneNetworks.Replace("[phoneNumber]", phoneNumber);
                var header  = await _transactionHelper.GetPGMRequestHeaders("NonBodyPGMRequest", url, Guid.NewGuid().ToString().Replace("-", ""));
                var RequestTime = DateTime.Now;
                var response = await RequestHelper.Get(url, header);
                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<Nine9psbGenResponse<PhoneNumberLookUpResponse>>(apiResponse);

                var log = new ExternalIntegrationLog
                {
                    CreatedBy = "Ascompay",
                    RequestTime = RequestTime,
                    RequestPayload = JsonConvert.SerializeObject(url),
                    Response = apiResponse,
                    ResponseTime = DateTime.Now,
                    Service = "Payment",
                    Vendor = "9PSB",
                };
                _appdbContext.ExternalIntegrationLogs.Add(log);
                _appdbContext.SaveChanges();

                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<PhoneNumberLookUpResponse>() { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<Nine9psbListGenResponse<DataPlansResponse>> GetDataPlans(string phoneNumber)
        {
            try
            {
                var responseObj = new Nine9psbListGenResponse<DataPlansResponse>();
                
                var url = NinePSBVatUrls.PGM_DataPlans.Replace("[phoneNumber]", phoneNumber);
                var header = await _transactionHelper.GetPGMRequestHeaders("NonBodyPGMRequest", url, Guid.NewGuid().ToString().Replace("-", ""));
                var response = await RequestHelper.Get(url, header);

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<Nine9psbListGenResponse<DataPlansResponse>>(apiResponse);
                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbListGenResponse<DataPlansResponse> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<Nine9psbGenResponse<StatusResponse>> GetTopUpStatus(string transactionReferenceId)
        {
            try
            {
                var responseObj = new Nine9psbGenResponse<StatusResponse>();
                var url = NinePSBVatUrls.PGM_TransactionStatus.Replace("[transReference]", transactionReferenceId);
                var header = await _transactionHelper.GetPGMRequestHeaders("NonBodyPGMRequest", url, Guid.NewGuid().ToString().Replace("-", ""));
                var response = await RequestHelper.Get(url, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<Nine9psbGenResponse<StatusResponse>>(apiResponse);

                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<StatusResponse> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<Nine9psbGenResponse<AirtimeTopUp>> PurchaseAirtime(AirTimeTopUpRequest requestModel)
        {
            try
            {
                var responseObj = new Nine9psbGenResponse<AirtimeTopUp>();

                StringContent content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");
                var header = await _transactionHelper.GetPGMRequestHeaders(JsonConvert.SerializeObject(requestModel), NinePSBVatUrls.PGM_AirTimePurchase);
                var RequestTime = DateTime.Now;
                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.PGM_AirTimePurchase, content, header);

                string apiResponse = await response.Content.ReadAsStringAsync();

                var log = new ExternalIntegrationLog
                {
                    CreatedBy = "Ascompay",
                    RequestTime = RequestTime,
                    RequestPayload = JsonConvert.SerializeObject(requestModel),
                    Response = apiResponse,
                    ResponseTime = DateTime.Now,
                    Service = "Payment",
                    Vendor = "9PSB",
                };
                _appdbContext.ExternalIntegrationLogs.Add(log);
                _appdbContext.SaveChanges();

                responseObj = JsonConvert.DeserializeObject<Nine9psbGenResponse<AirtimeTopUp>>(apiResponse);
                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<AirtimeTopUp> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<Nine9psbGenResponse<DataTopUpResponse>> PurchaseDataPlan(DataTopUpRequest requestModel)
        {
            try
            {
                var responseObj = new Nine9psbGenResponse<DataTopUpResponse>();
                var header = await _transactionHelper.GetPGMRequestHeaders(JsonConvert.SerializeObject(requestModel), NinePSBVatUrls.PGM_AirTimePurchase);
                var payLoad =  new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");
            //    StringContent content = new StringContent(JsonConvert.SerializeObject(payLoad), Encoding.UTF8, "application/json");

                var RequestTime = DateTime.Now;
                var response =  RequestHelper.PostWithBodyA(NinePSBVatUrls.PGM_DataPurchase, header, payLoad);               

                string apiResponse = await response.Content.ReadAsStringAsync();


                var log = new ExternalIntegrationLog
                {
                    CreatedBy = "Ascompay",
                    RequestTime = RequestTime,
                    RequestPayload = JsonConvert.SerializeObject(requestModel),
                    Response = apiResponse,
                    ResponseTime = DateTime.Now,
                    Service = "Payment",
                    Vendor = "9PSB",
                };
                _appdbContext.ExternalIntegrationLogs.Add(log);
                _appdbContext.SaveChanges();

                responseObj = JsonConvert.DeserializeObject<Nine9psbGenResponse<DataTopUpResponse>>(apiResponse);

                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<DataTopUpResponse> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        #endregion

        #region BILLER
        public async Task<Nine9psbListGenResponse<Category>> GetCategory()
        {
            try
            {
                var responseObj = new Nine9psbListGenResponse<Category>();

                var header = await _transactionHelper.GetPGMRequestHeaders("NonBodyPGMRequest", NinePSBVatUrls.PGM_BillCategories, Guid.NewGuid().ToString().Replace("-", ""));
                var response = await RequestHelper.Get(NinePSBVatUrls.PGM_BillCategories, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<Nine9psbListGenResponse<Category>>(apiResponse);

                return responseObj;

            }
            catch (Exception ex)
            {
                return new Nine9psbListGenResponse<Category> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<Nine9psbListGenResponse<CategoryBiller>> GetCategoryBiller(string categoryId)
        {
            try
            {
                var responseObj = new Nine9psbListGenResponse<CategoryBiller>();
                var header = await _transactionHelper.GetPGMRequestHeaders("NonBodyPGMRequest", NinePSBVatUrls.PGM_BillCategories, Guid.NewGuid().ToString().Replace("-", ""));
                var url = NinePSBVatUrls.PGM_CategoryBiller.Replace("[categoryId]", categoryId);
                var response = await RequestHelper.Get(url, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<Nine9psbListGenResponse<CategoryBiller>>(apiResponse);

                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbListGenResponse<CategoryBiller> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<Nine9psbListGenResponse<BillerField>> GetBillerInputFields(string billerId)
        {
            try
            {
                var responseObj = new Nine9psbListGenResponse<BillerField>();
                var header = await _transactionHelper.GetPGMRequestHeaders("NonBodyPGMRequest", NinePSBVatUrls.PGM_BillCategories, Guid.NewGuid().ToString().Replace("-", ""));
                var response = await RequestHelper.Get(NinePSBVatUrls.PGM_BillerInputFields.Replace("[billerId]", billerId), header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<Nine9psbListGenResponse<BillerField>>(apiResponse);
                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbListGenResponse<BillerField> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }

        public async Task<Nine9psbGenResponse<BillerFieldValidationResponse>> VaildateBillerInputFields(ValidateBillerInputRequest requestModel)
        {
            try
            {
                var responseObj = new Nine9psbGenResponse<BillerFieldValidationResponse>();

                var header =await  _transactionHelper.GetPGMRequestHeaders(JsonConvert.SerializeObject(requestModel), NinePSBVatUrls.PGM_BillerInputValidate, Guid.NewGuid().ToString().Replace("-", ""));
                StringContent content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");
                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.PGM_BillerInputValidate, content, header);
                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<Nine9psbGenResponse<BillerFieldValidationResponse>>(apiResponse);

                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<BillerFieldValidationResponse> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<Nine9psbGenResponse<BillerPaymentResponse>> InitBillerPayment(InitaiteBillPaymentRequest requestModel)
        {
            try
            {
                var responseObj = new Nine9psbGenResponse<BillerPaymentResponse>();

                var header = await _transactionHelper.GetPGMRequestHeaders(JsonConvert.SerializeObject(requestModel), NinePSBVatUrls.PGM_AirTimePurchase);
                StringContent content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");
                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.PGM_BillPayment, content, header);

                string apiResponse = await response.Content.ReadAsStringAsync();

                var log = new ExternalIntegrationLog
                {
                    CreatedBy = "Ascompay",
                    RequestTime = DateTime.Now,
                    RequestPayload = JsonConvert.SerializeObject(requestModel),
                    Response = apiResponse,
                    ResponseTime = DateTime.Now,
                    Service = "Payment",
                    Vendor = "9PSB",
                };
                _appdbContext.ExternalIntegrationLogs.Add(log);
                _appdbContext.SaveChanges();

                responseObj = JsonConvert.DeserializeObject<Nine9psbGenResponse<BillerPaymentResponse>>(apiResponse);

                return responseObj;
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<BillerPaymentResponse> { IsSuccessful = false, Message = ex.Message, Data = null };
            }
        }
        public async Task<Nine9psbGenResponse<TransactionStatusResponse>> GetBillerPaymentStatus(string transactionReferenceId)
        {
            try
            {
                var responseObj = new Nine9psbGenResponse<TransactionStatusResponse>();

                var header = await _transactionHelper.GetPGMRequestHeaders("NonBodyPGMRequest", NinePSBVatUrls.PGM_BillPaymentStatus.Replace("[transReference]", transactionReferenceId), Guid.NewGuid().ToString().Replace("-", ""));
                var url = NinePSBVatUrls.PGM_BillPaymentStatus.Replace("[transReference]", transactionReferenceId);
                var response = await RequestHelper.Get(url, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<Nine9psbGenResponse<TransactionStatusResponse>>(apiResponse);

                return responseObj; 
            }
            catch (Exception ex)
            {
                return new Nine9psbGenResponse<TransactionStatusResponse> { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }

        #endregion
    }
}
