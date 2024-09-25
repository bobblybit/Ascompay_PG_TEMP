using AscomPayPG.Data;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AscomPayPG.Services
{
    public class EncodeValue : IEncodeValue
    {

        public async Task<ResponseMessage> decrypt(string data, int type, Guid Token)
        {
            IConfiguration _configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

            string decryptUrl = _configuration["App:decrpt:url"];

            EntDecrpt word = new EntDecrpt()
            {
                Data = data,
                Type = type,
                Token = Token
            };

            ResponseMessage Apiresponse = new ResponseMessage();
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(word), Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.BaseAddress = new Uri(decryptUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsync(client.BaseAddress, content).Result;

                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Apiresponse = await Task.Run(() => JsonConvert.DeserializeObject<ResponseMessage>(apiResponse));
                    }
                    else
                    {
                        Apiresponse = new ResponseMessage()
                        {
                            isOk = false,
                            ResponseCode = 503,
                            Message = "Request Faied"
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                Apiresponse = new ResponseMessage()
                {
                    isOk = false,
                    ResponseCode = 500,
                    Message = ex.Message
                };
            }

            return Apiresponse;
        }


        public async Task<ResponseMessage> encrypt(string data, int type, Guid Token)
        {
            IConfiguration _configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

            string decryptUrl = _configuration["App:encrpt:url"];

            EntDecrpt word = new EntDecrpt()
            {
                Data = data,
                Type = type,
                Token = Token
            };

            ResponseMessage Apiresponse = new ResponseMessage();
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(word), Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.BaseAddress = new Uri(decryptUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsync(client.BaseAddress, content).Result;

                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Apiresponse = await Task.Run(() => JsonConvert.DeserializeObject<ResponseMessage>(apiResponse));
                    }
                    else
                    {
                        Apiresponse = new ResponseMessage()
                        {
                            isOk = false,
                            ResponseCode = 503,
                            Message = "Request Faied"
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                Apiresponse = new ResponseMessage()
                {
                    isOk = false,
                    ResponseCode = 500,
                    Message = ex.Message
                };
            }

            return Apiresponse;
        }

    }
}