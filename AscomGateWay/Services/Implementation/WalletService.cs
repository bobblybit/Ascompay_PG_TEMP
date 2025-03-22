using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Gateways;
using AscomPayPG.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AscomPayPG.Services.Implementation
{
    public class WalletService : IWalletService
    {
        private IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;
        private readonly ITransactionHelper _transactionHelper;
        private readonly ILogger<WalletService> _loggerWS;
        WAAS waas;

        public WalletService(IConfiguration configuration,
                            IClientRequestRepository<ClientRequest> clientRequestRepo,
                            AppDbContext context,
                            ITransactionHelper transactionHelper,
                             IServiceProvider serviceProvider,
                             ILogger<WalletService> loggerWS
                            )
        {
            _configuration = configuration;
            _context = context;
            _clientRequestRepo = clientRequestRepo;

            var logger = serviceProvider.GetRequiredService<ILogger<WAAS>>();


            _transactionHelper = transactionHelper;
            _loggerWS = loggerWS;
            waas = new WAAS(_configuration, context, _clientRequestRepo, _transactionHelper, logger);
        }
        public async Task<PlainResponse> AccessToken()
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.GetAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> OpenWallet(string userUid)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.OpenWallet(userUid);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }

        public async Task<PlainResponse> WalletUpgradeTier3multipart(WalletUpgradeTier3Request model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.UpgradeToTier3(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletUpgradeTier2(WalletUpgradeTier2Request model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.UpgradeToTier2(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }

        public async Task<PlainResponse> WalletEnquiry(WalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletEnquiry(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletStatus(WalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletStatus(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> GetWallet(WalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.GetWallet(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletTransactions(WalletTransactionRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletTransactions(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> ChangeWalletStatus(ChangeWalletStatusRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.ChangeWalletStatus(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletRequery(WalletRequeryRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletRequery(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletDebit(DebitWalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletDebit(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletCredit(CreditWalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletCredit(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }//-------> from contrller 1
        public async Task<PlainResponse> TransferOtherBank(OtherBankTransferDTO model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                var marchantCharge = await CalculateCharges(decimal.Parse(model.Amount), TransactionTypes.TransferToOthersBanks.ToString());
                var paymentProviderCharges = await GetPaymentProviderCharges(TransactionTypes.TransferToOthersBanks.ToString());
                var charges = paymentProviderCharges + marchantCharge;
                var vat = await _transactionHelper.CalculateVAT(decimal.Parse(model.Amount) + charges, TransactionTypes.TransferToOthersBanks.ToString());
                var transactionReference = Guid.NewGuid().ToString().Substring(0, 20).Replace("-", "").ToUpper();

                var sourceAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == model.senderAccountNumber);

                if (sourceAccount == null)
                {
                    return new PlainResponse
                    {
                        IsSuccessful = false,
                        Message = "sender account does not exist.",
                        Data = 0,
                    };
                }

                response = await waas.TransferOtherBank(model, false, true, transactionReference);

                if (!response.IsSuccessful)
                {
                    return new PlainResponse
                    {
                        IsSuccessful = false,
                        Message = response.Message,
                        Data = 0,
                    };
                }


                await UpdateSourceAccount(sourceAccount, decimal.Parse(model.Amount) + vat + charges);


                var debit = await _clientRequestRepo.BuildDebit(decimal.Parse(model.Amount), paymentProviderCharges, marchantCharge, model.RecieverName,
                                                                                      transactionReference, decimal.Parse(model.Amount) + vat + charges, model.Description,
                                                                                      TransactionTypes.TransferToOthersBanks.ToString(),
                                                                                      sourceAccount.AccountName,
                                                                                      vat, charges, PaymentProvider.AscomPay.ToString(),
                                                                                      sourceAccount.AccountNumber);
            }
            catch (Exception ex)
            {
                _loggerWS.LogInformation($"ERROR:::::::::::::::::::::::::::::::::::::::::{ex.Message} {ex.StackTrace}");

                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<UserWallet> GetIntenalWalletById(string Id) => await _context.UserWallets.FirstOrDefaultAsync(x => x.WalletUID.ToString() == Id);
        public async Task<bool> UpdateIntenalWallet(UserWallet walletToUpdate)
        {
             _context.UserWallets.Update(walletToUpdate);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateSourceAccount(Models.DTO.Account sourceAccount, decimal amount)
        {
            var previousBalnce = sourceAccount.CurrentBalance;
            sourceAccount.PrevioseBalance = previousBalnce;
            sourceAccount.CurrentBalance -= amount;
            sourceAccount.LegerBalance -= amount;
            _context.Accounts.Update(sourceAccount);
            await _context.SaveChangesAsync();
            return true;
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
    }
}

