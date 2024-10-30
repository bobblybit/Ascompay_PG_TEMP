using AscomPayPG.Helpers;
using AscomPayPG.Helpers.HTTPHelper;
using AscomPayPG.Helpers.HTTPHelper.UrlHelper;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.VasModels;
using AscomPayPG.Services.Gateways.Interface;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text;

namespace AscomPayPG.Services.Gateways.Implementation
{
    public class Vas9PSB : I9psbVaS
    {
        private async Task<VatAuthResponse> Authenticate()
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var userName = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVatConfiuration:Username");
                var password = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVatConfiuration:Password");

                var payload = new AuthenticationRequestModeL { username = userName, password = password };
                StringContent content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.Authenticate, content);

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                    if (responseObj?.code == "00")
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
        public async Task<PlainResponse> GetDataPlans(string phoneNumber)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                         Data = null,
                         IsSuccessful= false,
                         Message = tokenResponse.Token,
                         ResponseCode= 200
                    };
                }
                
                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };
                var response = await RequestHelper.Get(NinePSBVatUrls.DataPlans.Replace("[phoneNumber]", phoneNumber), header);

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                    if (responseObj?.code == "00")
                        return new PlainResponse {  IsSuccessful = true,  Message = responseObj.message, Data = responseObj.data };
                    else
                        return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> GetPhoneNetwork(string phoneNumber)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }

                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };
                var response = await RequestHelper.Get(NinePSBVatUrls.PhoneNetworks.Replace("[phoneNumber]", phoneNumber), header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> GetTopUpStatus(string transactionReferenceId)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }

                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };
                var response = await RequestHelper.Get(NinePSBVatUrls.TransactionStatus.Replace("[transReference]", transactionReferenceId), header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> PurchaseAirtime(AirTimeTopUpRequest requestModel)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }
                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };

                var payloadLoadAsJsonString = JsonConvert.SerializeObject(requestModel);
                StringContent content = new StringContent(JsonConvert.SerializeObject(payloadLoadAsJsonString), Encoding.UTF8, "application/json");

                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.AirTimePurchase, content, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> PurchaseDataPlan(DataTopUpRequest requestModel)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }
                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };

                var payloadLoadAsJsonString = JsonConvert.SerializeObject(requestModel);
                StringContent content = new StringContent(JsonConvert.SerializeObject(payloadLoadAsJsonString), Encoding.UTF8, "application/json");

                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.DataPurchase, content, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        #endregion

        #region BILLER
        public async Task<PlainResponse> GetCategory()
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }

                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };
                var response = await RequestHelper.Get(NinePSBVatUrls.BillCategories, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> GetCategoryBiller(string categoryId)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }

                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };
                var response = await RequestHelper.Get(NinePSBVatUrls.CategoryBiller.Replace("[categoryId]", categoryId), header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> GetBillerInputFields(string billerId)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }

                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };
                var response = await RequestHelper.Get(NinePSBVatUrls.BillerInputFields.Replace("[billerId]", billerId), header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> VaildateBillerInputFields(ValidateBillerInputRequest requestModel)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }
                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };

                var payloadLoadAsJsonString = JsonConvert.SerializeObject(requestModel);
                StringContent content = new StringContent(JsonConvert.SerializeObject(payloadLoadAsJsonString), Encoding.UTF8, "application/json");

                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.BillerInputValidate, content, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> InitBillerPayment(InitaiteBillPaymentRequest requestModel)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }
                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };

                var payloadLoadAsJsonString = JsonConvert.SerializeObject(requestModel);
                StringContent content = new StringContent(JsonConvert.SerializeObject(payloadLoadAsJsonString), Encoding.UTF8, "application/json");

                var response = await RequestHelper.PostWithBody(NinePSBVatUrls.BillPayment, content, header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }
        public async Task<PlainResponse> GetBillerPaymentStatus(string transactionReferenceId)
        {
            try
            {
                dynamic responseObj = new ExpandoObject();

                var tokenResponse = await Authenticate();
                if (!tokenResponse.IsSuccessfull)
                {
                    return new PlainResponse()
                    {
                        Data = null,
                        IsSuccessful = false,
                        Message = tokenResponse.Token,
                        ResponseCode = 200
                    };
                }

                var header = new Dictionary<string, string> { { "Authorization", $"Bearer {tokenResponse.Token}" } };
                var response = await RequestHelper.Get(NinePSBVatUrls.BillPaymentStatus.Replace("[transReference]", transactionReferenceId), header);

                string apiResponse = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);

                if (responseObj?.code == "00")
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
                else
                    return new PlainResponse { IsSuccessful = true, Message = responseObj.message, Data = responseObj.data };
            }
            catch (Exception ex)
            {
                return new PlainResponse { IsSuccessful = true, Message = ex.Message, Data = null };
            }
        }

        #endregion
    }
}
