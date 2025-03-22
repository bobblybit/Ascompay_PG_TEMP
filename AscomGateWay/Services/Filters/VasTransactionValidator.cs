    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Text;
    using System.IO;
    using System.Reflection;
    using AscomPayPG.Models.VasModels;

namespace AscomPayPG.Services.Filters
{
    public class VasTransactionValidator : IAuthorizationFilter
    {
        private readonly IHelperService _helperService; // Injected service

        public VasTransactionValidator(IHelperService helperService) // Inject dependency
        {
            _helperService = helperService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var transactionToken = request.Headers["token"];

           if (string.IsNullOrWhiteSpace(transactionToken))
            {
                context.Result = new UnauthorizedObjectResult("Token is required.");
                return;
            }

            if (request.ContentLength > 0 && request.Body.CanRead)
            {
                request.EnableBuffering(); // Allow multiple reads
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var body = reader.ReadToEndAsync().Result; // Read request body as a string
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        try
                        {
                            string transactionType = ExtractTransactionType(body); // Extract the transaction type
                            var requestModel = DeserializeRequest(transactionType, body);

                            if (requestModel == null)
                            {
                                context.Result = new BadRequestObjectResult("Invalid transaction type.");
                                return;
                            }

                            // Use reflection to extract required values 
                            string senderAccount = GetPropertyValue(requestModel, "debitAccount");
                            string receiverAccount = GetPropertyValue(requestModel, "phoneNumber") ?? GetPropertyValue(requestModel, "customerId");
                            string amount = GetPropertyValue(requestModel, "amount");

                            if (string.IsNullOrEmpty(senderAccount) || string.IsNullOrEmpty(receiverAccount) || string.IsNullOrEmpty(amount))
                            {
                                context.Result = new BadRequestObjectResult("Invalid request data.");
                                return;
                            }

                            // Validate the transaction using the extracted values
                            var response = _helperService.ValidateTransaction(transactionToken, senderAccount, receiverAccount, decimal.Parse(amount), transactionType).Result;

                            if (!response)
                            {
                                context.Result = new UnauthorizedObjectResult("Invalid transaction.");
                                return;
                            }

                            System.Diagnostics.Debug.WriteLine($"[Authorization] Parsed Request: {JsonSerializer.Serialize(requestModel)}");
                        }
                        catch (JsonException ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Authorization] Failed to deserialize request body: {ex.Message}");
                            context.Result = new BadRequestObjectResult("Invalid request format.");
                            return;
                        }
                    }

                    // Reset the stream position for further reading
                    request.Body.Position = 0;
                }
            }
        }

        private string ExtractTransactionType(string jsonBody)
        {
            if (string.IsNullOrWhiteSpace(jsonBody))
            {
                return null;
            }

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonBody))
                {
                    // Get the root object and check the properties
                    if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        // Instead of looking for "transactionType", determine the class name
                        var propertyNames = doc.RootElement.EnumerateObject().Select(p => p.Name).ToList();

                        // Check for known properties to infer the request type
                        if (propertyNames.Contains("phoneNumber") && propertyNames.Contains("network") && propertyNames.Contains("productId"))
                        {
                            return "DataTopUp";
                        }
                        else if (propertyNames.Contains("phoneNumber") && propertyNames.Contains("network"))
                        {
                            return "AirTimeTopUp";
                        }
                        else if (propertyNames.Contains("customerId") && propertyNames.Contains("billerId"))
                        {
                            return "Biller";
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error] Failed to parse JSON: {ex.Message}");
                return null;
            }

            return null;
        }
        private object DeserializeRequest(string transactionType, string body)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var typeMap = new Dictionary<string, Type>
        {
            { "AirTimeTopUp", typeof(AirTimeTopUpRequest) },
            { "DataTopUp", typeof(DataTopUpRequest) },
            /*{ "ValidateBiller", typeof(ValidateBillerInputRequest) },*/
            { "Biller", typeof(InitaiteBillPaymentRequest) }
        };

            return typeMap.ContainsKey(transactionType)
                ? JsonSerializer.Deserialize(body, typeMap[transactionType], options)
                : null;
        }
        private string GetPropertyValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return property?.GetValue(obj)?.ToString();
        }
    }

}
