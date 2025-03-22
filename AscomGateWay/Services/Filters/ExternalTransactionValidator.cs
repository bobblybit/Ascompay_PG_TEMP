using AscomPayPG.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using System.Text;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Data.Enum;

namespace AscomPayPG.Services.Filters
{
    public class ExternalTransactionValidator : IAuthorizationFilter
    {
        private readonly IHelperService _helperService; // Injected service

        public ExternalTransactionValidator(IHelperService helperService) // Inject dependency
        {
            _helperService = helperService;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var transactionToken = context.HttpContext.Request.Headers["token"];
            if (string.IsNullOrEmpty(transactionToken) || string.IsNullOrWhiteSpace(transactionToken))
            {
                context.Result = new UnauthorizedObjectResult("Token is required"); ;
                return;
            }


            if (request.ContentLength > 0 && request.Body.CanRead)
            {
                request.EnableBuffering(); // Allow multiple reads
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var body = reader.ReadToEndAsync().Result; // Read body as string
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        try
                        {
                            // Deserialize into a C# class (modify MyRequestModel to match your request structure)
                            var requestModel = JsonSerializer.Deserialize<OtherBankTransferDTO>(body, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true // Ignore case differences in JSON keys
                            });
                            // Do something with requestModel (e.g., logging, validation, etc.)
                            var response =  _helperService.ValidateTransaction(transactionToken,
                                                                                    requestModel.senderAccountNumber,
                                                                                    requestModel.RecieverNumber,
                                                                                    decimal.Parse(requestModel.Amount),
                                                                                    TransactionTypes.TransferToOthersBanks.ToString()
                                                                                   ).Result;
                            if (!response)
                            {
                                context.Result = new UnauthorizedObjectResult("invalid transaction."); ;
                                return;
                            }

                            System.Diagnostics.Debug.WriteLine($"[Authorization] Parsed Request: {JsonSerializer.Serialize(requestModel)}");
                        }
                        catch (JsonException ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Authorization] Failed to deserialize request body: {ex.Message}");
                        }
                    }
                    // Reset the stream position for further reading
                    request.Body.Position = 0;
                }
            }
        }
    }
}
