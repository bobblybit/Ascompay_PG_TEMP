using AscomPayPG.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common
{
    public static class SecurityHelper
    {
        private static readonly string encrypKey = "b14ca5898a4e4133bbce2ea2315a1917";
        private static readonly IConfiguration configuration = ConfigurationHelper.GetConfigurationInstance();

/*        public static ApiResponseMessage EncryptString(EntDecrpt key)
        {

            ApiResponseMessage Apiresult = new ApiResponseMessage();

            try
            {

                byte[] iv = new byte[16];
                byte[] array;
                string Skey = string.Empty;

                using (Aes aes = Aes.Create())
                {
                    if (key.Type == 0)
                    {
                        Skey = GetEncryptionKey();
                    }
                    else if (key.Type == 6789)
                    {
                        Skey = Convert.ToString(key.Token);
                    }
                    else
                    {
                        var Item = GetAdditionalSecurityKey(key.Type);


                        Skey = Item.Item1;

                        if (key.Token != Item.Item2)
                        {
                            Apiresult = new ApiResponseMessage()
                            {
                                Message = "Invalid Token",
                                ResponseCode = 400,
                                isOk = false
                            };
                        }
                    }

                    aes.Key = Encoding.UTF8.GetBytes(Skey);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(key.Data);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }


                Apiresult = new ApiResponseMessage()
                {
                    Message = Convert.ToBase64String(array),
                    ResponseCode = 200,
                    isOk = true
                };


            }
            catch (Exception ex)
            {
                Apiresult = new ApiResponseMessage()
                {
                    Message = ex.Message,
                    ResponseCode = 500,
                    isOk = false
                };
            }

            return Apiresult;
        }
*/
   /*     public static ApiResponseMessage DecryptString(EntDecrpt key)
        {
            ApiResponseMessage Apiresult = new ApiResponseMessage();

            try
            {

                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(key.Data);
                string Skey = string.Empty;

                using (Aes aes = Aes.Create())
                {
                    if (key.Type == 0)
                    {
                        Skey = GetEncryptionKey();
                    }
                    else if (key.Type == 6789)
                    {
                        Skey = Convert.ToString(key.Token);
                    }
                    else
                    {
                        var Item = GetAdditionalSecurityKey(key.Type);


                        Skey = Item.Item1;

                        if (key.Token != Item.Item2)
                        {
                            Apiresult = new ApiResponseMessage()
                            {
                                Message = "Invalid Token",
                                ResponseCode = 400,
                                isOk = false
                            };
                        }
                    }

                    aes.Key = Encoding.UTF8.GetBytes(Skey);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                Apiresult = new ApiResponseMessage()
                                {
                                    Message = streamReader.ReadToEnd(),
                                    ResponseCode = 200,
                                    isOk = true
                                };
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Apiresult = new ApiResponseMessage()
                {
                    Message = ex.Message,
                    ResponseCode = 500,
                    isOk = false
                };
            }

            return Apiresult;
        }*/

        public static string GetEncryptionKey()
        {
            var encryptionKey = Environment.GetEnvironmentVariable("AscomPSEncryptionKey");

            if (string.IsNullOrEmpty(encryptionKey))
            {
                // Log the error
                Console.WriteLine("Original key not found.");
                encryptionKey = encrypKey;
                Console.WriteLine("Alternative key used.");

                //throw new ApplicationException("Encryption key not found.");
            }

            return encryptionKey;
        }

        public static (string, Guid) GetAdditionalSecurityKey(int type)
        {
            string additionalSecurityKey = null;
            Guid additionalSecurityToken = Guid.Empty;

            var LKey = GetEncryptionKey();
            string Data = configuration.GetConnectionString("AppDB");
            var connectionString = DS(Data, LKey);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand($"SELECT eKey, Token FROM EnKeys WHERE Status = '1' And Type = {type}", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (LKey != null)
                            {
                                // Assuming SecurityKey is a property in SecurityKeyEntity.
                                additionalSecurityKey = reader["eKey"].ToString();
                                additionalSecurityToken = Guid.Parse(reader["Token"].ToString());
                            }
                            else
                            {
                                // Handle the case where the encryption key is not found.
                                throw new ApplicationException("Encryption key not found.");
                            }
                        }
                    }
                }
            }


            return (additionalSecurityKey, additionalSecurityToken);
        }

        public static string DS(string Data, string LKey)
        {
            try
            {

                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(Data);

                using (Aes aes = Aes.Create())
                {

                    aes.Key = Encoding.UTF8.GetBytes(LKey);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region ENCRYPTION/DECRYPTION METHODS OVERLOAD

       public static string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv)
        {
            string simpletext = String.Empty;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            simpletext = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return simpletext;
        }

        public static string SerializeObject<T>(T obj)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve // Handle object cycles
            };

            return JsonSerializer.Serialize(obj, options);
        }

        public static byte[] Encrypt(string simpletext, byte[] key, byte[] iv)
        {
            byte[] cipheredtext;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(simpletext);
                        }

                        cipheredtext = memoryStream.ToArray();
                    }
                }
            }
            return cipheredtext;
        }
        #endregion

        public static string CouponGenerator(int length)
        {
            char[] myArray = new char[] { 'a', '9', '8', '9', '7', '6', '5', '4', '3', '2', '1', '0', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
            var coupon = myArray.OrderBy(o => Guid.NewGuid()).Take(length);

            return new string(coupon.ToArray());
        }

    }
}
