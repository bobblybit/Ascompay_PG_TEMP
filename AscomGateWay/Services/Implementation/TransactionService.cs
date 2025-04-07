using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Helpers;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Gateways;
using AscomPayPG.Services.Interface;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace AscomPayPG.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;
        //private readonly IEmailNotification _emailNotification;
        private readonly ITransactionHelper _transactionHelper;
        //private readonly INotificationRepository _notificationRepository;
        private IConfiguration _configuration;
        private readonly AppDbContext _context;
        SmartObj smartObj;
        WAAS waas;
        private readonly WAAS _waas;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionService(IClientRequestRepository<ClientRequest> clientRequestRepo,
                                  IServiceProvider serviceProvider,
                                 ITransactionHelper transactionHelper, IConfiguration configuration, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _clientRequestRepo = clientRequestRepo;
            _transactionHelper = transactionHelper;
            _configuration = configuration;
            _context = context;
            smartObj = new SmartObj(_context);
            var logger = serviceProvider.GetRequiredService<ILogger<WAAS>>();
            waas = new WAAS(_configuration, context, _clientRequestRepo, _transactionHelper, logger);
            _httpContextAccessor = httpContextAccessor;
        }
/*        public async Task<TransferResponseDTO> TransferFundFromAccountOrWalletToAccount(TransferRequestDTO requestModel)
        {
            var destinationAccount = new Models.DTO.Account();
            var sourceAccount = new Models.DTO.Account();
            var sourceWallet = new UserWallet();
            var destinationWallet = new UserWallet();
            decimal senderNewBalance = 0;
            var sender = new User();
            var reciever = new User();

            var charges = await _transactionHelper.CalculateCharges(requestModel.Amount, TransactionTypes.TransferToAscomUsers.ToString());
            var vat = TransactionHelper.CalculateVAT(requestModel.Amount + charges);

            if (requestModel.SenderAccountOrWallet == requestModel.ReceiverAccount)
                return new TransferResponseDTO
                {
                    IsSuccessfull = false,
                    Message = "sender and reciever account cannot be the same",
                    SendNewBalance = 0,
                    TransferStatus = (int)TransferStatus.Failed,
                };

            if (requestModel.IsAccount)
            {

                sourceAccount = await _clientRequestRepo.GetUserAccount(requestModel.SenderAccountOrWallet);
                if (sourceAccount == null)
                {
                    var sourceWalletEntity = await _clientRequestRepo.GetWalletById(requestModel.SenderAccountOrWallet);
                    if (sourceWalletEntity != null)
                    {
                        var userUid = sourceWalletEntity.UserUid.ToString();
                        sourceAccount = await _clientRequestRepo.GetUserAccountByUserUid(userUid);
                        if (sourceAccount == null)
                            return new TransferResponseDTO
                            {
                                IsSuccessfull = false,
                                Message = "Source account does not exist",
                                SendNewBalance = 0,
                            };

                        if (sourceAccount.CurrentBalance < requestModel.Amount + charges + vat)
                            return new TransferResponseDTO
                            {
                                IsSuccessfull = false,
                                Message = "Insufficient Balance",
                                SendNewBalance = 0,
                            };
                        sender = await _clientRequestRepo.GetUser(sourceAccount.UserUid.ToString());
                    }
                }
                else
                {
                    sender = await _clientRequestRepo.GetUser(sourceAccount.UserUid.ToString());
                }
                destinationAccount = await _clientRequestRepo.GetUserAccount(requestModel.ReceiverAccount);
                if (destinationAccount == null)
                {
                    var sourceWalletEntity = await _clientRequestRepo.GetWalletById(requestModel.ReceiverAccount);
                    if (sourceWalletEntity != null)
                    {
                        var userUid = sourceWalletEntity.UserUid.ToString();
                        destinationAccount = await _clientRequestRepo.GetUserAccountByUserUid(userUid);
                    }
                    if (destinationAccount == null)
                    {
                        return new TransferResponseDTO
                        {
                            IsSuccessfull = false,
                            Message = "Destination account does not exist",
                            SendNewBalance = 0,
                            TransferStatus = (int)TransferStatus.Failed,
                        };
                    }
                }
                reciever = await _clientRequestRepo.GetUser(destinationAccount.UserUid.ToString());

                if (sender == null || sender.Username == null)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "Sender does not exist",
                        SendNewBalance = 0,
                    };

                if (sender.IsDeprecated)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "Sender has been deprecated",
                        SendNewBalance = 0,
                    };

                if (!sender.IsActive)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "sender is in-acctive",
                        SendNewBalance = 0,
                    };

                if (sourceWallet.CurrentBalance < requestModel.Amount + charges + vat)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "Insufficient Balance",
                        SendNewBalance = 0,
                    };

                var currentBalance = destinationAccount.CurrentBalance;
                destinationAccount.CurrentBalance += requestModel.Amount;
                destinationAccount.PrevioseBalance = currentBalance;
                bool response = await _clientRequestRepo.UpdateAccount(destinationAccount);
                if (!response)
                {
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "Transaction processing error",
                        SendNewBalance = 0,
                    };
                    throw new Exception("Transaction processing error");
                }
                else
                {
                    senderNewBalance = await UpdateSourceAccountBalance(sourceAccount, requestModel.Amount);
                    await NotifyForCredit($"{reciever.FirstName} {reciever.LastName}", reciever.Email,
                     $"{sender.FirstName} {sender.LastName}",
                     requestModel.Amount.ToString(),
                     destinationAccount.CurrentBalance.ToString(),
                     DateTime.Now.ToString(), requestModel.Decription);
                }
                var newTransaction = new Transactions()
                {
                    UserId = sender.UserId,
                    RequestTransactionId = string.IsNullOrEmpty(requestModel.transactionId) ? TransactionTypes.TransferToAscomUsers.ToString() + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff") : requestModel.transactionId,
                    UserUID = sender.UserUid,
                    Status = PaymentStatus.Approved.ToString(),
                    StatusId = 1,
                    Timestamp = DateTime.Now,
                    AccessToken = Guid.NewGuid().ToString(),
                    Amount = requestModel.Amount,
                    Email = !string.IsNullOrEmpty(sender.Email) ? sender.Email : string.Empty,
                    Description = requestModel.Decription,
                    TransactionType = TransactionTypes.TransferToAscomUsers.ToString(),
                    SourceAccount = requestModel.SenderAccountOrWallet,
                    DestinationAccount = requestModel.IsAccount ? requestModel.ReceiverAccount : null,
                    DestinationWallet = !requestModel.IsAccount ? requestModel.ReceiverAccount : null,
                    PaymentAction = PaymentActionType.Internal.ToString(),
                    BankCode = (int)BankCodes.Ascom,
                    T_Vat = vat,
                    T_Charge = charges
                };

                var transactionResponse = await _clientRequestRepo.AddTransaction(newTransaction);
                NotifyForDebit(sender.Email, $"{sender.FirstName} {sender.LastName}",
                   requestModel.Amount.ToString(), senderNewBalance.ToString(),
                   vat.ToString(), charges.ToString(), DateTime.Now.ToString(), requestModel.Decription);

                if (transactionResponse)
                {
                    //send mail
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = true,
                        Message = "Transfer was successfull",
                        SendNewBalance = senderNewBalance,
                    };
                }
                else
                {
                    var retriesResponse = await RetryAddTransaction(5, newTransaction);
                    if (retriesResponse)
                    {
                        //send mail
                        return new TransferResponseDTO
                        {
                            IsSuccessfull = true,
                            Message = "Transfer was successfull",
                            SendNewBalance = senderNewBalance,
                        };
                    }
                }
                throw new Exception("Something went wrong while processing transaction");

            }
            else
            {
                sourceWallet = await _clientRequestRepo.GetWalletById(requestModel.SenderAccountOrWallet);
                sender = await _clientRequestRepo.GetUser(sourceWallet.UserUid.ToString());
                if (sourceWallet == null)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "Source wallet does not exist",
                        SendNewBalance = 0,
                        TransferStatus = (int)TransferStatus.Failed
                    };
                if (requestModel.ReceiverAccount.Length == 10)
                {
                    destinationAccount = await _clientRequestRepo.GetUserAccount(requestModel.ReceiverAccount);
                    if (destinationAccount == null)
                    {
                        var sourceWalletEntity = await _clientRequestRepo.GetWalletById(requestModel.ReceiverAccount);
                        if (sourceWalletEntity != null)
                        {
                            var userUid = sourceWalletEntity.UserUid.ToString();
                            destinationAccount = await _clientRequestRepo.GetUserAccountByUserUid(userUid);
                        }
                        if (destinationAccount == null)
                        {
                            return new TransferResponseDTO
                            {
                                IsSuccessfull = false,
                                Message = "Destination account does not exist",
                                SendNewBalance = 0,
                                TransferStatus = (int)TransferStatus.Failed,
                            };
                        }
                    }
                    reciever = await _clientRequestRepo.GetUser(destinationAccount.UserUid.ToString());
                }
                else
                {
                    destinationWallet = await _clientRequestRepo.GetWalletById(requestModel.ReceiverAccount);
                    if (destinationWallet == null)
                    {
                        return new TransferResponseDTO
                        {
                            IsSuccessfull = false,
                            Message = "Destination wallet does not exist",
                            SendNewBalance = 0,
                            TransferStatus = (int)TransferStatus.Failed
                        };
                    }
                    reciever = await _clientRequestRepo.GetUser(destinationWallet.UserUid.ToString());
                }

                if (sender == null || sender.Username == null)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "Sender does not exist",
                        SendNewBalance = 0,
                    };

                if (sender.IsDeprecated)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "Sender has been deprecated",
                        SendNewBalance = 0,
                    };

                if (!sender.IsActive)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "sender is in-acctive",
                        SendNewBalance = 0,
                    };

                if (sourceWallet.CurrentBalance < requestModel.Amount + charges + vat)
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = false,
                        Message = "Insufficient Balance",
                        SendNewBalance = 0,
                    };
                if (requestModel.ReceiverAccount.Length == 10)
                {
                    var currentBalance = destinationAccount.CurrentBalance;
                    destinationAccount.CurrentBalance += requestModel.Amount;
                    destinationAccount.PrevioseBalance = currentBalance;
                    bool response = await _clientRequestRepo.UpdateAccount(destinationAccount);
                    if (!response)
                    {
                        return new TransferResponseDTO
                        {
                            IsSuccessfull = false,
                            Message = "Transaction processing error",
                            SendNewBalance = 0,
                        };
                        throw new Exception("Transaction processing error");
                    }
                    else
                    {
                        await NotifyForCredit($"{reciever.FirstName} {reciever.LastName}", reciever.Email,
                         $"{sender.FirstName} {sender.LastName}",
                         requestModel.Amount.ToString(),
                         destinationWallet.CurrentBalance.ToString(),
                         DateTime.Now.ToString(), requestModel.Decription);
                    }
                }
                else
                {
                    var currentBalance = destinationWallet.CurrentBalance;
                    destinationWallet.CurrentBalance += requestModel.Amount;
                    destinationWallet.PrevioseBalance = currentBalance;
                    bool response = await _clientRequestRepo.UpdateWallet(destinationWallet);
                    if (!response)
                    {
                        return new TransferResponseDTO
                        {
                            IsSuccessfull = false,
                            Message = "Transaction processing error",
                            SendNewBalance = 0,
                        };
                        throw new Exception("Transaction processing error");
                    }
                    else
                    {
                        await NotifyForCredit($"{reciever.FirstName} {reciever.LastName}", reciever.Email,
                         $"{sender.FirstName} {sender.LastName}",
                         requestModel.Amount.ToString(),
                         destinationWallet.CurrentBalance.ToString(),
                         DateTime.Now.ToString(), requestModel.Decription);
                    }
                }



                senderNewBalance = await UpdateSourceWalletBalance(sourceWallet, sourceAccount ,requestModel.Amount);

                var newTransaction = new Transactions()
                {
                    UserId = sender.UserId,
                    RequestTransactionId = string.IsNullOrEmpty(requestModel.transactionId) ? TransactionTypes.TransferToAscomUsers.ToString() + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff") : requestModel.transactionId,
                    UserUID = sender.UserUid,
                    Status = PaymentStatus.Approved.ToString(),
                    StatusId = 1,
                    Timestamp = DateTime.Now,
                    AccessToken = Guid.NewGuid().ToString(),
                    Amount = requestModel.Amount,
                    Email = !string.IsNullOrEmpty(sender.Email) ? sender.Email : string.Empty,
                    Description = requestModel.Decription,
                    TransactionType = TransactionTypes.TransferToAscomUsers.ToString(),
                    SourceAccount = requestModel.SenderAccountOrWallet,
                    DestinationAccount = requestModel.IsAccount ? requestModel.ReceiverAccount : null,
                    DestinationWallet = !requestModel.IsAccount ? requestModel.ReceiverAccount : null,
                    PaymentAction = PaymentActionType.Internal.ToString(),
                    BankCode = (int)BankCodes.Ascom,
                    T_Vat = vat,
                    T_Charge = charges
                };

                var transactionResponse = await _clientRequestRepo.AddTransaction(newTransaction);
                NotifyForDebit(sender.Email, $"{sender.FirstName} {sender.LastName}",
                   requestModel.Amount.ToString(), senderNewBalance.ToString(),
                   vat.ToString(), charges.ToString(), DateTime.Now.ToString(), requestModel.Decription, transactionReference);

                if (transactionResponse)
                {
                    //send mail
                    return new TransferResponseDTO
                    {
                        IsSuccessfull = true,
                        Message = "Transfer was successfull",
                        SendNewBalance = senderNewBalance,
                    };
                }
                else
                {
                    var retriesResponse = await RetryAddTransaction(5, newTransaction);
                    if (retriesResponse)
                    {
                        //send mail
                        return new TransferResponseDTO
                        {
                            IsSuccessfull = true,
                            Message = "Transfer was successfull",
                            SendNewBalance = senderNewBalance,
                        };
                    }
                }
                throw new Exception("Something went wrong while processing transaction");
            }



        }
*/
        

        private async Task<bool> RetryAddTransaction(int retries, Transactions dataToAdd)
        {
            bool isSuccessful = false;
            while (retries > 0 && !isSuccessful)
            {
                isSuccessful = await _clientRequestRepo.AddTransaction(dataToAdd);
                retries--;
            }
            return isSuccessful;
        }

        private async Task<decimal> GetUserTotalWalletBalance(string userId)
        {
            var userWallets =  _context.UserWallets.Where(x => x.UserUid.ToString() == userId).ToList();
            if (userWallets.Count() > 1)
            {
                return userWallets.Sum(uw => uw.CurrentBalance) ?? 0;
            }
            return 0;
        }

        public async Task<decimal> UpdateSourceAccountBalance(Models.DTO.Account account, decimal amount, bool isFromAccount = false)
        {
            var currentBalance = account.CurrentBalance;
            account.PrevioseBalance = currentBalance;
            account.CurrentBalance -= amount;
            var walletsBalance = await GetUserTotalWalletBalance(account.UserUid.ToString());
            account.LegerBalance -= amount;
            await _clientRequestRepo.UpdateAccount(account);
            return account.CurrentBalance ?? 0;
        }
        private async Task<decimal> UpdateDestinationAccountBalance(Models.DTO.Account account, decimal amount)
        {
            var currentBalance = account.CurrentBalance;
            account.CurrentBalance += amount;
            account.PrevioseBalance = currentBalance;
            account.LegerBalance += amount;
            await _clientRequestRepo.UpdateAccount(account);
            return account.CurrentBalance ?? 0;
        }
       
        private async Task<bool> UpdateSourceWalletBalance(UserWallet wallet, decimal amount)
        {
            var oldBalance = wallet.CurrentBalance;
            wallet.CurrentBalance -= amount;
            wallet.PrevioseBalance = oldBalance;
            return await _clientRequestRepo.UpdateWallet(wallet);
        }
        private async Task<bool> UpdateDestinationWalletBalance(UserWallet wallet, decimal amount)
        {
            var oldBalance = wallet.CurrentBalance;
            wallet.CurrentBalance += amount;
            wallet.PrevioseBalance = oldBalance;
            return await _clientRequestRepo.UpdateWallet(wallet);
        }

        public async Task<PlainResponse> WebhookReceiver(VirtualAccount_VM payload, string x_squad_signature)
        {
            PlainResponse response = new PlainResponse();
            Webhook webhook = new Webhook();
            bool isSignatureValid = false;
            try
            {

                isSignatureValid = SmartObj.ValidateWebhookSignature(payload, x_squad_signature);
                int webhookCnt = _context.Webhook.Count();
                if (isSignatureValid)
                {
                    var accountEntity = await _clientRequestRepo.GetUserAccount(payload.virtual_account_number);
                    decimal amount = Convert.ToDecimal(payload.principal_amount);
                    decimal newBalance = await UpdateSourceAccountBalance(accountEntity, amount);
                    var json = JsonSerializer.Serialize(payload);
                    response.IsSuccessful = true;
                    webhook.WebhookId = webhookCnt + 1;
                    webhook.Reference = payload.transaction_reference;
                    webhook.EventType = "collection.successful";
                    webhook.RequestString = json.ToString();
                    _context.Add(webhook);
                    await _context.SaveChangesAsync();
                    response.Message = $"web hook saved successfully.";
                    response.ResponseCode = StatusCodes.Status200OK;
                }
                else
                {
                    response.IsSuccessful = false;
                    response.Message = $"Failed to save web hook.";
                    response.ResponseCode = StatusCodes.Status400BadRequest;
                }
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
        public async Task<NinePSBWebhookResponse> WebhookReceiver9PSB(NinePSBWebhook payload)
        {
            NinePSBWebhookResponse response = new NinePSBWebhookResponse();
            Webhook webhook = new Webhook();
            try
            {

                var accountEntity = await _clientRequestRepo.GetUserAccount(payload.accountnumber);
                var hookEntity = _context.Webhook.FirstOrDefault(x => x.Reference == payload.transactionref);
                if (hookEntity == null)
                {
                    response.message = payload.message.ToLower();
                    response.success = true;
                    response.status = payload.message;
                    response.transactionref = payload.transactionref;
                    if (accountEntity != null)
                    {
                        decimal amount = Convert.ToDecimal(payload.amount);
                        decimal newBalance = await UpdateDestinationAccountBalance(accountEntity, amount);
                        //await RegisterCreditTransaction(payload.amount, payload.sendername);
                        var json = JsonSerializer.Serialize(payload);
                        webhook.Reference = payload.transactionref;
                        webhook.EventType = "collection.successful";
                        webhook.Vendor = "9PSB";
                        webhook.Service = "Payment";
                        webhook.RequestString = json.ToString();
                        _context.Add(webhook);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        decimal amount = Convert.ToDecimal(payload.amount);

                        var json = JsonSerializer.Serialize(payload);
                        webhook.Reference = payload.transactionref;
                        webhook.EventType = "collection.successful";
                        webhook.RequestString = json.ToString();
                        _context.Add(webhook);
                        await _context.SaveChangesAsync();
                    }

                    WalletRequest recipientWalletRequest = new WalletRequest()
                    {
                        accountNo = payload.accountnumber
                    };
                    PlainResponse getRecipientWallet = await waas.WalletEnquiry(recipientWalletRequest);

                    if (accountEntity != null)
                    {
                        var receiver = await _context.Users.FirstOrDefaultAsync(a => a.UserUid == accountEntity.UserUid);

                        await _transactionHelper.NotifyForCredit($"{receiver.FirstName} {receiver.LastName}", receiver.Email,
                     $"{payload.sendername}",
                     payload.amount.ToString(),
                     getRecipientWallet.Data.ledgerBalance.ToString(),
                     DateTime.Now.ToString(), payload.narration);
                        //send sms notification
                        await _transactionHelper.NotifyForCreditSMS(receiver, payload.accountnumber, payload.amount.ToString(), getRecipientWallet.Data.ledgerBalance.ToString(), payload.narration);


                        var paymentProviderCharges = await _transactionHelper.GetPaymentProviderCharges(TransactionTypes.IncommingTransfer.ToString());
                        var marchantCharge = await _transactionHelper.CalculateCharges(decimal.Parse(payload.amount), TransactionTypes.IncommingTransfer.ToString());
                        var charges = paymentProviderCharges + marchantCharge;
                        var vat = await _transactionHelper.CalculateVAT(decimal.Parse(payload.amount) + charges, TransactionTypes.IncommingTransfer.ToString());


                        var credit = await _clientRequestRepo.BuildCredit(decimal.Parse(payload.amount), paymentProviderCharges, marchantCharge,
                                                                           accountEntity.AccountName, accountEntity.UserUid.ToString(), payload.sendername, payload.transactionref,
                                                                          decimal.Parse(payload.amount) + vat + charges, payload.narration, TransactionTypes.IncommingTransfer.ToString(), vat, charges,
                                                                          PaymentProvider.AscomPay.ToString(), accountEntity.AccountNumber
                                                                          );

                        var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { credit });
                    }
                    else
                    {
                        response.message = payload.message;
                        response.success = true;
                        response.status = payload.message.ToLower();
                        response.transactionref = payload.transactionref;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.message = ex.Message;
                response.success = false;
                response.status = payload.message.ToLower();
                response.transactionref = payload.transactionref;
            }


            return response;
        }
        public async Task<PlainResponse> Banks()
        {
            PlainResponse response = new PlainResponse();
            try
            {
                var banks = await smartObj.GetBanks();
                if (banks.Count() > 0)
                {
                    response.Data = banks;
                    response.IsSuccessful = true;
                    response.Message = "Banks List";
                    response.ResponseCode = StatusCodes.Status200OK;
                }

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

        public async Task<AccountLookUpResponse> AccountLookup(accountLookupRequest accountLookupRequest, string userUId)
            {
            var response = new AccountLookUpResponse();
            try
            {
                return await waas.AccountLookup9PSB(accountLookupRequest, userUId);

                /*var appUser = _context.Users.Where(x => x.UserUid == Guid.Parse(userUid)).FirstOrDefault();
                if (appUser == null)
                {
                    response.Data = null;
                    response.IsSuccessful = false;
                    response.Message = "Invalid user";
                    response.ResponseCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                var resp = await smartObj.AccountLookup(accountLookupRequest);
                if (response.ResponseCode == StatusCodes.Status200OK)
                {
                    response.Data = resp.Data;
                    response.IsSuccessful = resp.IsSuccessful;
                    response.Message = resp.Message;
                    response.ResponseCode = resp.ResponseCode;
                }
                else
                {
                    response.Data = resp.Data;
                    response.IsSuccessful = resp.IsSuccessful;
                    response.Message = resp.Message;
                    response.ResponseCode = resp.ResponseCode;
                }*/
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
        public async Task<PlainResponse> TransferFund(FundTransfer model, string userUid)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                var appUser = _context.Users.Where(x => x.UserUid == Guid.Parse(userUid)).FirstOrDefault();
                if (appUser == null)
                {
                    response.Data = null;
                    response.IsSuccessful = false;
                    response.Message = "Invalid user";
                    response.ResponseCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                var resp = await smartObj.TransferFund(model);
                if (response.ResponseCode == StatusCodes.Status200OK)
                {
                    response.Data = resp.Data;
                    response.IsSuccessful = resp.IsSuccessful;
                    response.Message = resp.Message;
                    response.ResponseCode = resp.ResponseCode;
                }
                else
                {
                    response.Data = resp.Data;
                    response.IsSuccessful = resp.IsSuccessful;
                    response.Message = resp.Message;
                    response.ResponseCode = resp.ResponseCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }//-----> controller 2
        public async Task<PlainResponse> TransferFundInternal(TransferRequestDTO requestModel)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                decimal paymentProviderCharges = 0;
                if (requestModel.TransactionType.ToLower() == TransactionTypes.TransferToAscomUsers.ToString().ToLower())
                {
                    paymentProviderCharges = await _transactionHelper.GetPaymentProviderCharges(requestModel.TransactionType);
                }
                var marchantCharge = await _transactionHelper.CalculateCharges(requestModel.Amount, requestModel.TransactionType);
                var charges = paymentProviderCharges + marchantCharge;
                var vat = await _transactionHelper.CalculateVAT(requestModel.Amount + charges, requestModel.TransactionType);
                string transactionReference = Guid.NewGuid().ToString().Substring(0, 20).Replace("-", "").ToUpper();
                var sourceAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == requestModel.SenderAccountOrWallet);
                var receiverWallet = await _context.UserWallets.FirstOrDefaultAsync(x => x.WalletUID.ToString() == requestModel.ReceiverAccountOrWallet);
                var recieverAccount = await _clientRequestRepo.GetAccount(requestModel.ReceiverAccountOrWallet);
                var sourceWallet = await _clientRequestRepo.GetWalletById(requestModel.SenderAccountOrWallet);
               
                var sender = new User();
                var reciever = new User();

                string lookUpId = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<string>("lookUp");
                var lookUpRecord = _context.AccountLookUpLog
                                           .OrderByDescending(x => x.DateCreated)
                                           .FirstOrDefault(x => lookUpId == x.LookUpId
                                                            && x.LookStatus == true
                                                            && x.UsageStatus == (int)AccountLookUpUsageStatus.Init
                                                            );

                if (lookUpRecord == null)
                {
                    return new PlainResponse
                    {
                        IsSuccessful = false,
                        Message = "invalid receiver",
                        Data = 0,
                    };
                }



                #region ASCOM ACCOUNT TO WALLET
                if (requestModel.TransactionType == TransactionTypes.AscomPayAccountToWallet.ToString())
                {
                    var senderAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == requestModel.SenderAccountOrWallet);

                    if (senderAccount == null)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Sender account does exist",
                            Data = 0,
                        };
                    }

                    sender = await _clientRequestRepo.GetUser(senderAccount.UserUid.ToString());
                    if (sender == null)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "sender does not exist",
                            Data = 0,
                        };
                    }

                    if (receiverWallet == null)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Receiver wallet does exist",
                            Data = 0,
                        };
                    }

                    if (sourceAccount.UserUid != receiverWallet.UserUid)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "You can tranfer only to your wallet",
                            Data = 0,
                        };
                    }

                    if (requestModel.Amount < 50)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Transfer amount cannot be less than 50 Naira",
                            Data = 0,
                        };
                    }

                    if (sourceAccount.CurrentBalance < requestModel.Amount + charges + vat)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = " Insufficient Balance",
                            Data = 0,
                        };
                    }

                    // registar transaction
                    var regResponse = await _clientRequestRepo.RegisterTransaction(requestModel.Amount, paymentProviderCharges, marchantCharge, receiverWallet.WalletName,
                                                                                   sender, transactionReference, requestModel.Amount + vat + charges, requestModel.Decription,
                                                                                   requestModel.TransactionType, vat, charges, PaymentProvider.AscomPay.ToString(),
                                                                                   senderAccount.AccountNumber, "", "", receiverWallet.WalletUID.ToString());
                    if (!regResponse)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Something went wrong while performing transaction",
                            Data = 0,
                        };
                    }

                    var fundReponse = await FundWalletFromAccount(receiverWallet, sourceAccount, requestModel.Amount + charges + vat);

                    if (!fundReponse)
                    {
                        await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Failed.ToString());

                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Something went wrong while performing transaction",
                            Data = 0,
                        };
                    }


                    var credit = await _clientRequestRepo.BuildCredit(requestModel.Amount, paymentProviderCharges, marchantCharge,
                                                                      receiverWallet.WalletName, receiverWallet.UserUid.ToString(), senderAccount.AccountName, $"{transactionReference}",
                                                                      requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType, vat, charges,
                                                                      PaymentProvider.AscomPay.ToString(), receiverWallet.WalletUID.ToString()
                                                                      );

                    var debit = await _clientRequestRepo.BuildDebit(requestModel.Amount, paymentProviderCharges, marchantCharge, receiverWallet.WalletName,
                                                                    $"{transactionReference}", requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType,
                                                                    senderAccount.AccountName,
                                                                    vat, charges, PaymentProvider.AscomPay.ToString(),
                                                                    sourceAccount.UserUid.ToString(),
                                                                    sourceAccount.AccountNumber
                                                                    );

                    var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { debit, credit });
                    var updateResponse = await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Successful.ToString());

                    return new PlainResponse
                    {
                        IsSuccessful = true,
                        Message = "Transaction Was completed Succesfully",
                        Data = new { },
                        transaction_reference = transactionReference,
                        ResponseCode = 200
                    };

                }
                #endregion

                #region WALLET TO WALLET
                if (requestModel.TransactionType == TransactionTypes.WalletToWalletOnline.ToString())
                {

                    if (sourceWallet == null)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Sender Wallet does exist",
                            Data = 0,
                        };
                    }

                    if (receiverWallet == null)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Receiver wallet does exist",
                            Data = 0,
                        };
                    }

                    if (sourceWallet.WalletUID == receiverWallet.WalletUID)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "source wallet and receiver wallet can not be the same.",
                            Data = 0,
                        };
                    }

                    if (requestModel.Amount < 50)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Transfer amount cannot be less than 50 Naira",
                            Data = 0,
                        };
                    }

                    if (sourceWallet.CurrentBalance < requestModel.Amount + charges)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Insuffient balance",
                            Data = 0,
                        };
                    }
                    sender = await _clientRequestRepo.GetUser(sourceWallet?.UserUid.ToString());
                    // registar transaction
                    var regResponse = await _clientRequestRepo.RegisterTransaction(requestModel.Amount, paymentProviderCharges, marchantCharge, receiverWallet.WalletName,
                                                                                   sender, transactionReference, requestModel.Amount + vat + charges,
                                                                                   requestModel.Decription, requestModel.TransactionType, vat, charges,
                                                                                   PaymentProvider.AscomPay.ToString(), "", "", sourceWallet.WalletUID.ToString(),
                                                                                   receiverWallet.WalletUID.ToString());

                    // registar transaction
                    var debitResponse = await UpdateSourceWalletBalance(sourceWallet, requestModel.Amount + charges + vat);
                    if (!debitResponse)
                    {
                        await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Failed.ToString());
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "something went wrong while performing trasaction",
                            Data = 0,
                        };
                    }

                    var creditResponse = await UpdateDestinationWalletBalance(receiverWallet, requestModel.Amount + charges + vat);
                    if (!creditResponse)
                    {
                        await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Failed.ToString());
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "something went wrong while performing trasaction",
                            Data = 0,
                        };
                    }

                    var credit = await _clientRequestRepo.BuildCredit(requestModel.Amount, paymentProviderCharges, marchantCharge,
                                                                      receiverWallet.WalletName, receiverWallet.UserUid.ToString(), sourceWallet.WalletName, transactionReference,
                                                                     requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType, vat, charges,
                                                                     PaymentProvider.AscomPay.ToString(), receiverWallet.WalletUID.ToString());

                    var debit = await _clientRequestRepo.BuildDebit(requestModel.Amount, paymentProviderCharges, marchantCharge, receiverWallet.WalletName,
                                                                        transactionReference, requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType,
                                                                        sourceWallet.WalletName,
                                                                        vat, charges, PaymentProvider.AscomPay.ToString(),
                                                                        sourceWallet.UserUid.ToString(),
                                                                        sourceWallet.WalletUID.ToString());
                    
                    var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { debit, credit });
                    var updateResponse = await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Successful.ToString());

                    return new PlainResponse
                    {
                        IsSuccessful = true,
                        Message = "Transaction Was completed Succesfully",
                        Data = new { },
                        transaction_reference = transactionReference,
                        ResponseCode = 200
                    };
                }
                #endregion

                #region WALLET TO ASCOM ACCOUNT
                               if (requestModel.TransactionType == TransactionTypes.WalletToAscomPayAccount.ToString())
                               {

                                   if (sourceWallet == null)
                                   {
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Sender Wallet does exist",
                                           Data = 0,
                                       };
                                   }


                                   if (recieverAccount == null)
                                   {
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Receiver Account does exist",
                                           Data = 0,
                                       };
                                   }


                                if (requestModel.Amount < 50)
                                {
                                    return new PlainResponse
                                    {
                                        IsSuccessful = false,
                                        Message = "Transfer amount cannot be less than 50 Naira",
                                        Data = 0,
                                    };
                                }

                    sender = await _clientRequestRepo.GetUser(sourceWallet.UserUid.ToString());
                                   if (sourceWallet.CurrentBalance < requestModel.Amount + charges + vat)
                                   {
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Insufficient Balance",
                                           Data = 0,
                                       };
                                   }


                                    if (recieverAccount.UserUid != sourceWallet.UserUid)
                                    {
                                            
                                            var senderAccount = await _clientRequestRepo.GetUserAccountByUserUid(sourceWallet.UserUid.ToString());

                                            var TransferRequest9BSB = new OtherBankTransferDTO
                                           {
                                               Amount = requestModel.Amount.ToString(),
                                               bank = "9BSP",
                                               Description = requestModel.Decription,
                                               Narration = requestModel.Decription,
                                               UserId = sender.UserUid.ToString(),
                                               senderAccountNumber = senderAccount.AccountNumber,
                                               senderName = senderAccount.AccountName,
                                           };

                                            response = await waas.TransferOtherBank(TransferRequest9BSB, lookUpRecord.AccountNumber, lookUpRecord.AccountName, true, false, transactionReference);

                                            if (!response.IsSuccessful)
                                            {
                                                return new PlainResponse
                                                {
                                                    IsSuccessful = false,
                                                    Message = response.Message,
                                                    Data = 0,
                                                };
                                            }

                                           await FundAccountFromWalletExternal(sourceWallet, senderAccount, recieverAccount, requestModel.Amount + charges + vat);
                                    }
                                    else
                                    {
                                        // registar transaction
                                        var regResponse = await _clientRequestRepo.RegisterTransaction(requestModel.Amount, paymentProviderCharges, marchantCharge, requestModel.ReceiverAccountName,
                                                                                                       sender, transactionReference, requestModel.Amount + vat + charges,
                                                                                                       requestModel.Decription, requestModel.TransactionType, vat, charges, PaymentProvider.AscomPay.ToString(),
                                                                                                       "", recieverAccount.AccountNumber, sourceWallet.WalletUID.ToString(), "");

                                        var fundReponse = await FundAccountFromWalletSelf(sourceWallet, recieverAccount, requestModel.Amount + charges);

                                        if (!fundReponse)
                                        {
                                            await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Failed.ToString());

                                            return new PlainResponse
                                            {
                                               IsSuccessful = false,
                                               Message = "Something went wrong while performing transaction",
                                               Data = 0,
                                            };
                                         }

                                        var updateResponse = await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Successful.ToString());
                                    }


                                   var credit = await _clientRequestRepo.BuildCredit(requestModel.Amount, paymentProviderCharges, marchantCharge,
                                                                                      recieverAccount.AccountName, recieverAccount.UserUid.ToString(), sourceWallet.WalletName, transactionReference,
                                                                                     requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType, vat, charges,
                                                                                     PaymentProvider.AscomPay.ToString(), recieverAccount.AccountNumber
                                                                                     );

                                   var debit = await _clientRequestRepo.BuildDebit(requestModel.Amount, paymentProviderCharges, marchantCharge, recieverAccount.AccountName,
                                                                                   transactionReference, requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType,
                                                                                   sourceWallet.WalletName,
                                                                                   vat, charges, PaymentProvider.AscomPay.ToString(),
                                                                                   sourceWallet.UserUid.ToString(),
                                                                                   sourceWallet.WalletUID.ToString()
                                                                                   );

                                   var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { debit, credit });

                                   return new PlainResponse
                                   {
                                       IsSuccessful = true,
                                       Message = "Transaction Was completed Succesfully",
                                       Data = new { },
                                       transaction_reference = transactionReference,
                                       ResponseCode = 200
                                   };

                               }
                               #endregion

                #region ASCOMPAY TO ASCOMPAY ACCOUNT
                               if (requestModel.TransactionType == TransactionTypes.TransferToAscomUsers.ToString())
                                {
                                   recieverAccount = await _context.Accounts.Include(x => x.AccountTeir).FirstOrDefaultAsync( x => x.AccountNumber == requestModel.ReceiverAccountOrWallet);

                                   if (recieverAccount == null)
                                   {
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Receiver account does exist",
                                           Data = 0,
                                       };
                                   }

                                   reciever = await _clientRequestRepo.GetUser(recieverAccount.UserUid.ToString());
                                   decimal senderNewBalance = 0;
                                   sourceAccount = await _clientRequestRepo.GetUserAccount(requestModel.SenderAccountOrWallet);

                                   if (sourceAccount == null)
                                   {
                                           return new PlainResponse
                                           {
                                               IsSuccessful = false,
                                               Message = "source account account does exist",
                                               Data = 0,
                                           };
                                   }
                                   sender = await _clientRequestRepo.GetUser(sourceAccount.UserUid.ToString());

                                   if (sender == reciever && requestModel.TransactionType == TransactionTypes.TransferToAscomUsers.ToString())
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "sender and reciever account cannot be the same",
                                           Data = 0,
                                       };


                            if (requestModel.Amount < 50)
                            {
                                return new PlainResponse
                                {
                                    IsSuccessful = false,
                                    Message = "Transfer amount cannot be less than 50 Naira",
                                    Data = 0,
                                };
                            }

                            if (sourceAccount.CurrentBalance < (requestModel.Amount + charges + vat))
                                               return new PlainResponse
                                               {
                                                   IsSuccessful = false,
                                                   Message = "Insufficient Balance",
                                                   Data = 0,
                                               };


                                   var TransferRequest9BSB = new OtherBankTransferDTO
                                   {
                                       Amount = requestModel.Amount.ToString(),
                                       bank = "9BSP",
                                       Description = requestModel.Decription,
                                       Narration = requestModel.Decription,
                                       UserId = sourceAccount.UserUid.ToString(),
                                       senderAccountNumber = sourceAccount.AccountNumber,
                                       senderName = sourceAccount.AccountName
                                   };

                                   response = await waas.TransferOtherBank(TransferRequest9BSB, lookUpRecord.AccountNumber, lookUpRecord.AccountName, true, true, transactionReference);

                                   if (!response.IsSuccessful)
                                   {
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Something went wrong while performing transaction",
                                           Data = 0,
                                       };
                                   }


                                  await UpdateSoruceAccountForAscomToAscomAccount(sourceAccount, requestModel.Amount + charges + vat);

                                   var credit = await _clientRequestRepo.BuildCredit(requestModel.Amount, paymentProviderCharges, marchantCharge,
                                                                                      recieverAccount.AccountName, recieverAccount.UserUid.ToString(), sourceAccount.AccountName, transactionReference,
                                                                                     requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType, vat, charges,
                                                                                     PaymentProvider.AscomPay.ToString(), recieverAccount.AccountNumber
                                                                                     );

                                  var debit = await _clientRequestRepo.BuildDebit(requestModel.Amount, paymentProviderCharges, marchantCharge, recieverAccount.AccountName,
                                                                                       transactionReference, requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType,
                                                                                       sourceAccount.AccountName,
                                                                                       vat, charges, PaymentProvider.AscomPay.ToString(),
                                                                                       sourceAccount.UserUid.ToString(),
                                                                                       sourceAccount.AccountNumber);

                                var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { debit, credit });
                                 /*WalletRequest senderWalletRequest = new WalletRequest()
                                   {
                                       accountNo = sourceAccount.AccountNumber
                                   };

                                   PlainResponse getSenderWallet = await waas.WalletEnquiry(senderWalletRequest);

                                   WalletRequest recipientWalletRequest = new WalletRequest()
                                   {
                                       accountNo = recieverAccount.AccountNumber
                                   };

                                   PlainResponse getRecipientWallet = await waas.WalletEnquiry(recipientWalletRequest);*/

                                 /*  _transactionHelper.NotifyForDebit(sender.Email, $"{sender.FirstName} {sender.LastName}",
                                      requestModel.Amount.ToString(), getSenderWallet.Data.ledgerBalance.ToString(),
                                      vat.ToString(), charges.ToString(), DateTime.Now.ToString(), requestModel.Decription, transactionReference);

                                       await _transactionHelper.NotifyForCredit($"{reciever.FirstName} {reciever.LastName}", reciever.Email,
                                        $"{sender.FirstName} {sender.LastName}",
                                        requestModel.Amount.ToString(),
                                        getRecipientWallet.Data.ledgerBalance.ToString(),
                                        DateTime.Now.ToString(), requestModel.Decription);

                                   //send sms notification
                                   await _transactionHelper.NotifyForDebitSMS(sender, sourceAccount.AccountNumber, requestModel.Amount.ToString(), getSenderWallet.Data.ledgerBalance.ToString(), requestModel.Decription);


                                   await _transactionHelper.NotifyForCreditSMS(reciever, recieverAccount.AccountNumber, requestModel.Amount.ToString(), getRecipientWallet.Data.ledgerBalance.ToString(), requestModel.Decription);*/

                                   return new PlainResponse
                                   {
                                       IsSuccessful = true,
                                       Message = "Transaction Was completed Succesfully",
                                       Data = new { },
                                       transaction_reference = transactionReference,
                                       ResponseCode = 200
                                   };
                               }
                               #endregion

                #region WALLET TO EXTERNAL
                               if (requestModel.TransactionType == TransactionTypes.WalletToExternalOnline.ToString())
                               {
                                   // first check if wallet exists
                                   if (sourceWallet == null)
                                   {
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Sender Wallet does exist",
                                           Data = 0,
                                       };
                                   }

                                   //check if balance is surfficient
                                   if (sourceWallet.CurrentBalance < requestModel.Amount + charges)
                                   {
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Insufficient Balance",
                                           Data = 0,
                                       };
                                   }

                                   sender = await _clientRequestRepo.GetUser(sourceWallet.UserUid.ToString());
                                   var senderAccount = await _context.Accounts.Include(x => x.AccountTeir).FirstOrDefaultAsync(x => x.UserUid == sender.UserUid);

                                   var TransferRequest9BSB = new OtherBankTransferDTO
                                   {
                                       Amount = requestModel.Amount.ToString(),
                                       bank = "9BSP",
                                       Description = requestModel.Decription,
                                       Narration = requestModel.Decription,
                                       UserId = sender.UserUid.ToString(),
                                       senderAccountNumber = senderAccount.AccountNumber,
                                       senderName = senderAccount.AccountName
                                   };

                                   response = await waas.TransferOtherBank(TransferRequest9BSB, lookUpRecord.AccountNumber, lookUpRecord.AccountName, true, false, transactionReference);

                                   if (!response.IsSuccessful)
                                   {
                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Something went wrong while performing transaction",
                                           Data = 0,
                                       };
                                   }
                                   // debit wallet 
                                  var walletBalanceResponse =  await UpdateSourceWalletBalance(sourceWallet, requestModel.Amount);

                                   if (!walletBalanceResponse)
                                   {
                                       await _clientRequestRepo.UpdateTransactionStatusByReference(transactionReference, PaymentStatus.Failed.ToString());

                                       return new PlainResponse
                                       {
                                           IsSuccessful = false,
                                           Message = "Something went wrong while performing transaction",
                                           Data = 0,
                                       };
                                   }

                                   var credit = await _clientRequestRepo.BuildCredit(requestModel.Amount, paymentProviderCharges, marchantCharge,
                                                                                      recieverAccount.AccountName, recieverAccount.UserUid.ToString(), sourceAccount.AccountName, transactionReference,
                                                                                     requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType, vat, charges,
                                                                                     PaymentProvider.AscomPay.ToString(), recieverAccount.AccountNumber
                                                                                     );

                                     var debit = await _clientRequestRepo.BuildDebit(requestModel.Amount, paymentProviderCharges, marchantCharge, recieverAccount.AccountName,
                                                                                       transactionReference, requestModel.Amount + vat + charges, requestModel.Decription, requestModel.TransactionType,
                                                                                       sourceAccount.AccountName,
                                                                                       vat, charges, PaymentProvider.AscomPay.ToString(),
                                                                                       sourceAccount.AccountNumber);

                                   var journalResponse = await _clientRequestRepo.SaveTransactionJournal(new List<TransactionJournal> { debit, credit });
                                   return new PlainResponse
                                   {
                                       IsSuccessful = true,
                                       Message = "Transaction Was completed Succesfully",
                                       Data = new { },
                                       transaction_reference = transactionReference,
                                       ResponseCode = 200
                                   };
                               }
                               #endregion

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

       public async Task<bool> FundWalletFromAccount(UserWallet receiverWallet, Models.DTO.Account sourceAccount, decimal amount)
        {
            if (sourceAccount.CurrentBalance < amount)
                return false;
            await UpdateDestinationWalletBalance(receiverWallet, amount);
            var previousBalnce = sourceAccount.CurrentBalance;
            sourceAccount.PrevioseBalance= previousBalnce;
            sourceAccount.CurrentBalance -= amount;
            _context.Accounts.Update(sourceAccount);
            await _context.SaveChangesAsync();
            return true;
        }
       public async Task<bool> UpdateSoruceAccountForAscomToAscomAccount(Models.DTO.Account sourceAccount, decimal amount)
       {
            var previousBalnce = sourceAccount.CurrentBalance;
            sourceAccount.PrevioseBalance = previousBalnce;
            sourceAccount.CurrentBalance -= amount;
            sourceAccount.LegerBalance -= amount;
            _context.Accounts.Update(sourceAccount);
            await _context.SaveChangesAsync();
            return true;
        }
       public async Task<bool> FundAccountFromWalletSelf(UserWallet sourceWallet, Models.DTO.Account receiveAccount, decimal amount)
       {
            await UpdateSourceWalletBalance(sourceWallet, amount);
            var oldBalance = receiveAccount.CurrentBalance;
            receiveAccount.CurrentBalance += amount;
            receiveAccount.PrevioseBalance = oldBalance;
            _context.Accounts.Update(receiveAccount);
            await _context.SaveChangesAsync();
            return true;
        }
       public async Task<bool> FundAccountFromWalletExternal(UserWallet sourceWallet, Models.DTO.Account senderAccount, Models.DTO.Account receiveAccount, decimal amount)
        {
            try
            {
                await UpdateSourceWalletBalance(sourceWallet, amount);
                // update source account
                senderAccount.LegerBalance -= amount;
                _context.Accounts.Update(senderAccount);
                await _context.SaveChangesAsync();
              /*  // update destination account
                var oldBalance = receiveAccount.CurrentBalance;
                receiveAccount.CurrentBalance += amount;
                receiveAccount.PrevioseBalance = oldBalance;
                receiveAccount.LegerBalance+= amount;   
                _context.Accounts.Update(receiveAccount);
                await _context.SaveChangesAsync();
*/
                return true;

            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


        public Task<bool> SettleOfflineWith9PSB()
        {
            // get all offline transaction transactions  where has settled = false, and status is successful
           //  
            throw new NotImplementedException();
        }

        //public async void RegisterCreditTransaction(decimal amount, string receiverAccountName, string receiverAccount, Models.DTO.Account senderAccount,  string description)
        //{
        //    // registar transaction
        //    var sender = await _clientRequestRepo.GetUser(senderAccount.UserUid.ToString());
        //    var transactionReference = Guid.NewGuid().ToString().Substring(0, 20).Replace("-", "").ToUpper();
        //    var regResponse = await _clientRequestRepo.RegisterTransaction(amount, 0, 0, receiverAccountName,
        //                                                                   sender, transactionReference, amount + 0 + 0, description,
        //                                                                   TransactionTypes.Credit.ToString(), 0, 0, PaymentProvider.NinePSB.ToString(),
        //                                                                   senderAccount.AccountNumber, receiverAccount, "", "",true);
        //}
    }
}

