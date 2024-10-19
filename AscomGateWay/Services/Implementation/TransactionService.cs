using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Data.Repository.Interface;
using AscomPayPG.Helpers;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.DTOs;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Interface;
using Microsoft.EntityFrameworkCore;
using SERVICES.Helpers;
using SERVICES.Services.Interface.Notification;
using System.Text.Json;

namespace AscomPayPG.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;
        private readonly IEmailNotification _emailNotification;
        private readonly ITransactionHelper _transactionHelper;
        private readonly INotificationRepository _notificationRepository;
        private IConfiguration _configuration;
        private readonly AppDbContext _context;
        SmartObj smartObj;
        WAAS waas;

        public TransactionService(IClientRequestRepository<ClientRequest> clientRequestRepo,
                                 IEmailNotification emailNotification,
                                 INotificationRepository notificationRepository,
                                 ITransactionHelper transactionHelper, IConfiguration configuration, AppDbContext context)
        {
            _clientRequestRepo = clientRequestRepo;
            _emailNotification = emailNotification;
            _transactionHelper = transactionHelper;
            _notificationRepository = notificationRepository;
            _configuration = configuration;
            _context = context;
            smartObj = new SmartObj(_context);
            waas = new WAAS(_configuration, context, _clientRequestRepo);
        }
        public async Task<TransferResponseDTO> TransferFundFromAccountOrWalletToAccount(TransferRequestDTO requestModel)
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



        }

        private async Task NotifyForCredit(string receiverFullName, string receiverEmail, string sender, string amount, string balance, string transactionTime, string decription)
        {
            string errorMessage = string.Empty;
            var emailObject = new TransactionAlertNotificationDTO
            {
                Amount = amount,
                Balance = balance,
                User = receiverFullName,
                Sender = sender,
                Time = transactionTime,
                Description = decription
            };

            var htmlContent = EmailComposer.ComposeCreditOrDebitNotificationHtmlContent(emailObject, true);
            try
            {
                await _emailNotification.NewSendEmail("Ascom Pay", "Transaction Notification", htmlContent, receiverEmail);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                //log notification

                throw;
            }
            finally
            {
                var notificationToAdd = new NotificationLog
                {
                    DCreatedOn = DateTime.Now,
                    DLastTriedOn = DateTime.Now,
                    IHasAttachment = false,
                    ITryCount = 5,
                    SRecipient = receiverEmail,
                    SMessage = htmlContent,
                    Sender = "Ascom",
                    SComment = errorMessage == string.Empty ? "Sent" : $"Not sent {errorMessage}",
                    SSubject = "Transaction Notification",
                    IStatus = 1,
                    SAttachmentCount = 0,
                    Origin = "AscomPayPG",
                };

                var responsed = await _notificationRepository.AddNotification(notificationToAdd);
            }
        }

        private void NotifyForDebit(string mailReciever, string receiverFullName, string amount, string balance, string vat, string charges, string transactionTime, string description)
        {
            var errorMessage = string.Empty;
            var emailObject = new TransactionAlertNotificationDTO
            {
                Amount = amount,
                Balance = balance,
                User = receiverFullName,
                Time = transactionTime,
                Vat = vat,
                Charges = charges,
                Description = description
            };
            var htmlContent = EmailComposer.ComposeCreditOrDebitNotificationHtmlContent(emailObject, false);
            try
            {
                _emailNotification.NewSendEmail("Ascom Pay", "Transaction Notification", htmlContent, mailReciever);
            }
            catch (Exception ex)
            {
                //log notification
                errorMessage = ex.Message;
                throw;
            }
            finally
            {
                var notificationToAdd = new NotificationLog
                {
                    DCreatedOn = DateTime.Now,
                    DLastTriedOn = DateTime.Now,
                    IHasAttachment = false,
                    ITryCount = 5,
                    SRecipient = mailReciever,
                    SMessage = htmlContent,
                    Sender = "Ascom",
                    SComment = errorMessage == string.Empty ? "Sent" : $"Not sent {errorMessage}",
                    SSubject = "Transaction Notification",
                    IStatus = 1,
                    SAttachmentCount = 0,
                    Origin = "AscomPayPG",
                };
                _notificationRepository.AddNotification(notificationToAdd);
            }
        }

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

        public async Task<decimal> UpdateSourceAccountBalance(Models.DTO.Account account, decimal amount)
        {
            var currentBalance = account.CurrentBalance;
            account.CurrentBalance -= amount;
            account.PrevioseBalance = currentBalance;
            var walletsBalance = await GetUserTotalWalletBalance(account.UserUid.ToString());
            account.LegerBalance = account.CurrentBalance + walletsBalance;
            await _clientRequestRepo.UpdateAccount(account);
            return account.CurrentBalance ?? 0;
        }
        private async Task<decimal> UpdateDestinationAccountBalance(Models.DTO.Account account, decimal amount)
        {
            var currentBalance = account.CurrentBalance;
            account.CurrentBalance += amount;
            account.PrevioseBalance = currentBalance;
            var walletsBalance = await GetUserTotalWalletBalance(account.UserUid.ToString());
            account.LegerBalance = account.CurrentBalance + walletsBalance;
            await _clientRequestRepo.UpdateAccount(account);
            return account.CurrentBalance ?? 0;
        }
        private async Task<decimal> UpdateSourceWalletBalance(UserWallet wallet, Models.DTO.Account sourceAccount, decimal amount)
        {
            var currentBalance = wallet.CurrentBalance;
            wallet.CurrentBalance -= amount;
            wallet.PrevioseBalance = currentBalance;
            await _clientRequestRepo.UpdateWallet(wallet);

            sourceAccount.LegerBalance -= amount;    
           await _clientRequestRepo.UpdateAccount(sourceAccount);

            return wallet.CurrentBalance ?? 0;
        }

        private async Task<decimal> UpdateSourceExternaleWalletBalance(UserExternalWallet wallet, decimal amount)
        {
            var old = wallet.availableBalance;
            var currentBalance = decimal.Parse(old) - amount;
            wallet.availableBalance = currentBalance.ToString();
            await _clientRequestRepo.UpdateExternalWallet(wallet);
            return decimal.Parse(wallet.availableBalance);
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

        public async Task<PlainResponse> WebhookReceiver9PSB(NinePSBWebhook payload)
        {
            PlainResponse response = new PlainResponse();
            Webhook webhook = new Webhook();
            try
            {

                var accountEntity = await _clientRequestRepo.GetUserAccount(payload.accountnumber);
                var hookEntity = await _context.Webhook.FirstOrDefaultAsync(x => x.Reference == payload.transactionref);
                if(hookEntity == null)
                {
                    if (accountEntity != null)
                    {
                        decimal amount = Convert.ToDecimal(payload.amount);
                        decimal newBalance = await UpdateDestinationAccountBalance(accountEntity, amount);
                        var json = JsonSerializer.Serialize(payload);
                        response.IsSuccessful = true;
                        webhook.Reference = payload.transactionref;
                        webhook.EventType = "collection.successful";
                        webhook.Vendor = "9PSB";
                        webhook.Service = "Payment";
                        webhook.RequestString = json.ToString();
                        _context.Add(webhook);
                        await _context.SaveChangesAsync();
                        response.Message = $"web hook saved successfully.";
                        response.ResponseCode = StatusCodes.Status200OK;
                    }
                    else
                    {
                        decimal amount = Convert.ToDecimal(payload.amount);

                        var json = JsonSerializer.Serialize(payload);
                        response.IsSuccessful = true;
                        webhook.Reference = payload.transactionref;
                        webhook.EventType = "collection.successful";
                        webhook.RequestString = json.ToString();
                        _context.Add(webhook);
                        await _context.SaveChangesAsync();
                        response.Message = $"web hook saved successfully.";
                        response.ResponseCode = StatusCodes.Status200OK;
                    }
                }
                else
                {
                    response.IsSuccessful = false;
                    response.Message = "Webhook notification already received";
                    response.ResponseCode = StatusCodes.Status409Conflict;
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
        public async Task<PlainResponse> AccountLookup(accountLookupRequest accountLookupRequest, string userUid)
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
        }
        public async Task<PlainResponse> TransferFundFromAccountOrWalletToAccount9PSB(TransferRequestDTO requestModel)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                var charges = await _transactionHelper.CalculateCharges(requestModel.Amount, TransactionTypes.TransferToAscomUsers.ToString());
                var vat = TransactionHelper.CalculateVAT(requestModel.Amount + charges);

                var sourceAccount = new Models.DTO.Account();
                var recieverAccount = await _clientRequestRepo.GetUserAccount(requestModel.ReceiverAccount);
                var sourceWallet = await _clientRequestRepo.GetWalletById(requestModel.SenderAccountOrWallet);
                var sender = new User();
                var reciever = new User();

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
                if (requestModel.IsAccount)
                {
                    sourceAccount =  await _clientRequestRepo.GetUserAccount(requestModel.SenderAccountOrWallet);
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
                }
                else
                {
                    if (sourceWallet == null)
                    {
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "source wallet account does exist",
                            Data = 0,
                        };
                    }
                    sender = await _clientRequestRepo.GetUser(sourceWallet.UserUid.ToString());
                    sourceAccount = await _clientRequestRepo.GetUserAccountByUserUid(sender.UserUid.ToString());
                }


                /*if (sender == reciever)
                    return new PlainResponse
                    {
                        IsSuccessful = false,
                        Message = "sender and reciever account cannot be the same",
                        Data = 0,
                    };*/

                if (requestModel.IsAccount)
                {
                    if (sourceAccount.CurrentBalance < requestModel.Amount + charges + vat)
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Insufficient Balance",
                            Data = 0,
                        };
                }
                else
                {
                    if (sourceWallet.CurrentBalance < requestModel.Amount + charges + vat)
                        return new PlainResponse
                        {
                            IsSuccessful = false,
                            Message = "Insufficient Balance",
                            Data = 0,
                        };
                }


                var TransferRequest9BSB = new OtherBankTransferDTO
                {
                    Amount = requestModel.Amount.ToString(),
                    bank = "9BSP",
                    Description = requestModel.Decription,
                    Narration = requestModel.Decription,
                    RecieverName = recieverAccount.AccountName,
                    RecieverNumber = recieverAccount.AccountNumber,
                    UserId = sourceAccount.UserUid.ToString(),
                    senderAccountNumber = sourceAccount.AccountNumber,
                    senderName = sourceAccount.AccountName
                };

                response  = await waas.TransferOtherBank(TransferRequest9BSB);

                if (response.IsSuccessful)
                {
                    await UpdateDestinationAccountBalance(recieverAccount, requestModel.Amount);
                    if (requestModel.IsAccount)
                    {
                        senderNewBalance = await  UpdateSourceAccountBalance(sourceAccount, requestModel.Amount);
                    }
                    else
                    {
                        senderNewBalance = await UpdateSourceWalletBalance(sourceWallet, sourceAccount, requestModel.Amount);
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
                        PaymentAction = PaymentActionType.Internal9PSB.ToString(),
                        BankCode = (int)BankCodes.Ascom,
                        T_Vat = vat,
                        T_Charge = charges
                    };

                    var transactionResponse = await _clientRequestRepo.AddTransaction(newTransaction);
                   
                    NotifyForDebit(sender.Email, $"{sender.FirstName} {sender.LastName}",
                   requestModel.Amount.ToString(), senderNewBalance.ToString(),
                   vat.ToString(), charges.ToString(), DateTime.Now.ToString(), requestModel.Decription);

                    await NotifyForCredit($"{reciever.FirstName} {reciever.LastName}", reciever.Email,
                     $"{sender.FirstName} {sender.LastName}",
                     requestModel.Amount.ToString(),
                     recieverAccount.CurrentBalance.ToString(),
                     DateTime.Now.ToString(), requestModel.Decription);
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
    }
}

