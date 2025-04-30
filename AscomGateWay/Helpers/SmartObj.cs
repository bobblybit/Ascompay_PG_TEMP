using AscomPayPG.Data;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace AscomPayPG.Helpers
{
    public class SmartObj
    {
        private static readonly IConfiguration _configuration = ConfigurationHelper.GetConfigurationInstance();
        private readonly AppDbContext _context;

        public SmartObj(AppDbContext context)
        {
            _context = context;
            
        }


        public static bool ValidateWebhookSignature(VirtualAccount_VM payload, string x_squad_signature)
        {
            bool isValid = false;
            string signature = string.Empty;
            try
            {
                if(payload == null)
                {
                    return isValid;
                }
                else
                {
                    signature = x_squad_signature;
                }
                string secretKey = _configuration["GTBConfiguration:Authorization"].ToString();

                string SerializedPayload = JsonConvert.SerializeObject(payload);
                //Console.WriteLine(SerializedPayload);
                string result = "";
                var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
                var inputBytes = Encoding.UTF8.GetBytes(SerializedPayload);
                var hmac = new HMACSHA512(secretKeyBytes);
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                //EncryptAesManaged(SerializedPayload);
                result = BitConverter.ToString(hashValue).Replace("-", string.Empty);
                result = result.ToLower();
                if(result.Equals(signature))
                {
                    isValid = true;
                    return isValid;
                }
            }
            catch
            {
                return isValid;
            }
            return isValid;
        }

        static void EncryptAesManaged(string raw)
        {
            try
            {
                // Create Aes that generates a new key and initialization vector (IV).    
                // Same key must be used in encryption and decryption    
                using (AesManaged aes = new AesManaged())
                {
                    // Encrypt string    
                    byte[] encrypted = Encrypt(raw, aes.Key, aes.IV);
                    // Print encrypted string    
                    Console.WriteLine($"Encrypted data: {System.Text.Encoding.UTF8.GetString(encrypted)}");
                    // Decrypt the bytes to a string.    
                    string decrypted = Decrypt(encrypted, aes.Key, aes.IV);
                    // Print decrypted string. It should be same as raw data    
                    Console.WriteLine($"Decrypted data: {decrypted}");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            Console.ReadKey();
        }
        public static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }
        public static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        public async Task<List<Bank>> GetBanks()
        {
            var bankList = await _context.Banks.ToListAsync();
            return bankList;
        }

        public async Task<PlainResponse> AccountLookup(accountLookupRequest accountLookupRequest)
        {
            PlainResponse respObj = new PlainResponse();
            try
            {
                string fullUrl = string.Empty;
                string baseUrl = _configuration["GTBConfiguration:BaseUrl"].ToString();
                string secretKey = _configuration["GTBConfiguration:Authorization"].ToString();

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");
                    fullUrl = $"{baseUrl}/payout/account/lookup";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(accountLookupRequest), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            respObj.IsSuccessful = true;
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Data = responseObj.data;
                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.ResponseCode = (int)response.StatusCode;
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
        public async Task<PlainResponse> TransferFund(FundTransfer model)
        {
            PlainResponse respObj = new PlainResponse();
            try
            {
                string merchantId = _configuration["GTBConfiguration:MerchantId"].ToString();
                model.transaction_reference = $"{merchantId}_{GenRefNo()}";
                model.remark = string.IsNullOrEmpty(model.remark) ? $"Transfer of {model.amount} to {model.account_name} ({model.account_number})" : model.remark;
                string fullUrl = string.Empty;
                string baseUrl = _configuration["GTBConfiguration:BaseUrl"].ToString();
                string secretKey = _configuration["GTBConfiguration:Authorization"].ToString();

                dynamic responseObj = new ExpandoObject();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");
                    fullUrl = $"{baseUrl}/payout/transfer";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(fullUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            respObj.IsSuccessful = true;
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.Data = responseObj.data;
                            respObj.ResponseCode = (int)response.StatusCode;
                            respObj.Message = responseObj.message;
                        }
                        else
                        {
                            respObj.IsSuccessful = false;
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            responseObj = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                            respObj.ResponseCode = (int)response.StatusCode;
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
        public static bool VerifyPasswordHash(string password, byte[]? passwordHash, byte[]? passwordSalt)
        {
            using HMACSHA512 hmac = new HMACSHA512(passwordSalt);
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static string GetServerLocalIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString(); // IPv4
                }
            }

            return null;
        }

        public static string GenRefNo()
        {
            string result = string.Empty;
            result = DateTime.UtcNow.Ticks.ToString();
            return result;
        }

    }
}
