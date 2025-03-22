using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Offline;
using AscomPayPG.OfflineSecurity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using AscomPayPG.Data.Enum;
using AscomPayPG.OfflineSecurity.DTO;
using AscomPayPG.Models;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Models.Shared;
using System.Security.Cryptography;
using Common;
using Newtonsoft.Json;

namespace AscomPayPG.Services.Interface
{
    public class OfflineSettlementService : IOfflineSettlementService
    {
        private readonly AppDbContext _appContext;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<IOfflineSettlementService> _logger;

        public OfflineSettlementService(AppDbContext appDbContext, ITransactionService transactionService, ILogger<IOfflineSettlementService> logger)
        {
            _appContext = appDbContext;
            _transactionService = transactionService;
            _logger = logger;
        }

        public decimal CalculateVAT(decimal amount, int transactionId)
        {
            var transactionTypeDetails = _appContext.TransactionType.FirstOrDefault(x => x.TiD == transactionId);
            if (transactionTypeDetails != null)
            {
                if (transactionTypeDetails.Add_Vat)
                {
                    return Math.Round((decimal)transactionTypeDetails.T_Vat / 100 * amount, 1);
                }
                return 0;
            }
            return 0;
        }
        public async Task<decimal> CalculateCharges(decimal amount, int transactionType)
        {
            var transactionTypeDetails = _appContext.TransactionType.FirstOrDefault(x => x.TiD == transactionType);
            if (transactionTypeDetails != null)
            {

                if (transactionTypeDetails.By_Percent)
                    return Math.Round((decimal)transactionTypeDetails.T_Percentage / 100 * amount, 1);
                else if (transactionTypeDetails.By_Amount)
                    return transactionTypeDetails.T_Amount;
                else return 0;
            }
            return 0;
        }


        private async Task<List<OfflineTransaction>> RemoveSettledTransactions(List<OfflineTransaction> transactions)
        {
            var groupedTransactions = transactions.GroupBy(t => t.TransactionId).Select(g => g.First()).ToList();
            var UniqueTransactionId = groupedTransactions.Select(x => x.TransactionId).Distinct().ToList();

            var existingTransactions = _appContext.Transactions.Where(t => UniqueTransactionId.Contains(t.RequestTransactionId)).ToList(); /* await _appContext.Transactions.Where(t => groupedTransactions.Any(ut => ut.TransactionId == t.RequestTransactionId)).ToListAsync();*/

            foreach (var existingTransaction in existingTransactions)
            {
                groupedTransactions.RemoveAll(t => t.TransactionId == existingTransaction.RequestTransactionId);
            }
            //unsettled transactions
            return groupedTransactions;
        }
        private async Task<UnSettledTransactionWithBalance> SettleTransactions(List<OfflineTransaction> transactionToSettled)
        {
            decimal Total = 0;

            var response = new UnSettledTransactionWithBalance();

            var failedSttlement = new List<OfflineTransaction>();

            foreach (var transaction in transactionToSettled)
            {
                Transactions transactionModel = new Transactions();
                // source wallet
                var debitWallet = await _appContext.UserWallets.FirstOrDefaultAsync(x => x.WalletUID.ToString() == transaction.SenderWalletId);

                if (debitWallet == null)
                {
                    transactionModel.StatusId = (int)TransactionStatus.Failed;
                    transactionModel.Status = TransactionStatus.Failed.ToString();
                    failedSttlement.Add(transaction);
                    continue;
                }
                else
                {
                    if (debitWallet.CurrentBalance < transaction.TransactionsAmount)
                    {
                        transactionModel.StatusId = (int)TransactionStatus.Failed;
                        transactionModel.Status = TransactionStatus.Failed.ToString();
                        failedSttlement.Add(transaction);
                        continue;

                    }
                    else if (!debitWallet.IsActive)
                    {
                        transactionModel.StatusId = (int)TransactionStatus.Failed;
                        transactionModel.Status = TransactionStatus.Failed.ToString();
                        failedSttlement.Add(transaction);
                        continue;

                    }
                    else if (debitWallet.IsDeprecated)
                    {
                        transactionModel.StatusId = (int)TransactionStatus.Failed;
                        transactionModel.Status = TransactionStatus.Failed.ToString();
                        failedSttlement.Add(transaction);
                        continue;
                    }
                    else
                    {
                        transactionModel.StatusId = (int)TransactionStatus.Success;
                        transactionModel.Status = TransactionStatus.Success.ToString();
                    }
                }

                var recieverWallet = await _appContext.UserWallets.FirstOrDefaultAsync(x => x.WalletUID.ToString() == transaction.ReceiverWalletId);

                if (recieverWallet == null || recieverWallet.IsDeprecated || !recieverWallet.IsActive)
                {
                    transactionModel.StatusId = (int)TransactionStatus.Failed;
                    transactionModel.Status = TransactionStatus.Failed.ToString();
                    failedSttlement.Add(transaction);
                    continue;
                }
                else
                {
                    transactionModel.StatusId = (int)TransactionStatus.Success;
                    transactionModel.Status = TransactionStatus.Success.ToString();
                }

                var charges = await CalculateCharges(transaction.TransactionsAmount, (int)TransactionTypes.AccountToWalletTopUp);
                var vat = CalculateVAT(transaction.TransactionsAmount + charges, (int)TransactionTypes.AccountToWalletTopUp);

                transactionModel.UserId = transaction.SenderIntegerId;
                transactionModel.UserUID = new Guid(transaction.SenderUid);
                transactionModel.RequestTransactionId = transaction.TransactionId;
                transactionModel.TransactionType = TransactionTypes.OfflineTransaction.ToString();
                transactionModel.SourceWallet = transaction.SenderWalletId;
                transactionModel.DestinationWallet = transaction.ReceiverWalletId;
                transactionModel.TransactionStatus = transaction.TransactionStatus;
                transactionModel.Amount = transaction.TransactionsAmount;
                transactionModel.Timestamp = transaction.TransactionDate;
                transactionModel.T_Vat = vat;
                transactionModel.T_Charge = charges;
                transactionModel.PaymentAction = "Internal";
                transactionModel.Currency = "NGN";

                #region REMOTE SETTLEMENT

                if (transactionModel.Status == TransactionStatus.Success.ToString())
                {
                    var sourceAccount = _appContext.Accounts.Include(x => x.AccountTeir).FirstOrDefault(x => x.UserUid == debitWallet.UserUid);
                    var receiverAccount = _appContext.Accounts.Include(x => x.AccountTeir).FirstOrDefault(x => x.UserUid == recieverWallet.UserUid);

                    var transactionToPost = new TransferRequestDTO
                    {
                        Amount = transaction.TransactionsAmount,
                        Decription = transaction.Description,
                        transactionId = transaction.TransactionId,
                        TransactionType = TransactionTypes.TransferToAscomUsers.ToString(),
                        SenderAccountOrWallet = sourceAccount.AccountNumber,
                        ReceiverAccountOrWallet = receiverAccount.AccountNumber,
                        ReceiverAccountName = receiverAccount.AccountName,
                    };

                    var responseFromPosting = await _transactionService.TransferFundInternal(transactionToPost);
                    var  objAsString = JsonConvert.SerializeObject(responseFromPosting);
                    _logger.LogInformation($"TRANSFER RESPONSE::::::::::::::::{objAsString}");

                    if (responseFromPosting.IsSuccessful && transactionModel.Status == TransactionStatus.Success.ToString())
                    {
                        _logger.LogInformation($"TRANSACTION STATUS:::::::::::::::::::::::::::::::{transactionModel.Status} TRANSACTION REFERENCE:::::::::::::::::::::::::::::::::::{transactionModel.RequestTransactionId}");
                        var settlementResponse = await SettleBalance(sourceAccount, receiverAccount, debitWallet, recieverWallet, (decimal)transactionModel.Amount);
                        if (!settlementResponse)
                        {
                            var transactionRetry = new TransactionRetry
                            {
                                HasbeenSettled = false,
                                IsOffline = false,
                                Amount = transaction.TransactionsAmount,
                                Decription = transaction.Description,
                                transactionId = transaction.TransactionId,
                                TransactionType = TransactionTypes.TransferToAscomUsers.ToString(),
                                SenderAccountOrWallet = sourceAccount.AccountNumber,
                                ReceiverAccountOrWallet = receiverAccount.AccountNumber,
                                ReceiverAccountName = receiverAccount.AccountName,
                                OfflineId = transactionModel.RequestTransactionId,
                                ReceiverWalletId = transactionModel.DestinationWallet,
                                SenderWalletId = transactionModel.SourceWallet,
                                hasPostedSuccessfully = true,
                            };

                            _appContext.TransactionRetries.Add(transactionRetry);
                            _appContext.SaveChanges();
                        }
                    }
                    else
                    {
                        var transactionRetry = new TransactionRetry
                        {
                            HasbeenSettled = false,
                            IsOffline = false,
                            Amount = transaction.TransactionsAmount,
                            Decription = transaction.Description,
                            transactionId = transaction.TransactionId,
                            TransactionType = TransactionTypes.TransferToAscomUsers.ToString(),
                            SenderAccountOrWallet = sourceAccount.AccountNumber,
                            ReceiverAccountOrWallet = receiverAccount.AccountNumber,
                            ReceiverAccountName = receiverAccount.AccountName,
                            OfflineId = transactionModel.RequestTransactionId,
                            ReceiverWalletId = transactionModel.DestinationWallet,
                            SenderWalletId = transactionModel.SourceWallet
                        };

                        _appContext.TransactionRetries.Add(transactionRetry);
                        _appContext.SaveChanges();

                        // update balances
                        /* if (debitWallet != null)
                         {
                             var debitwalletPlaceHolder = debitWallet.CurrentBalance;
                             debitWallet.CurrentBalance -= transaction.TransactionsAmount;
                             debitWallet.PrevioseBalance = debitwalletPlaceHolder;
                             _appContext.UserWallets.Update(debitWallet);
                         }

                        //credit destination 
                         if (recieverWallet != null)
                         {
                             var walletPlaceHolder = recieverWallet.CurrentBalance;
                             recieverWallet.CurrentBalance += transaction.TransactionsAmount;
                             recieverWallet.PrevioseBalance = walletPlaceHolder;
                              _appContext.UserWallets.Update(recieverWallet);
                         }*/
                    }

                }
                #endregion

                Total = (decimal)transactionModel.Amount++;
                _appContext.Transactions.Add(transactionModel);
                await _appContext.SaveChangesAsync();
                response.Balance = (decimal)debitWallet.CurrentBalance;
            }

            // Save changes to the database
            await _appContext.SaveChangesAsync();
            response.UnSettledTransaction = failedSttlement;
            return response;
        }
        private async Task<bool> SettleBalance(Models.DTO.Account sourceAccount, Models.DTO.Account receiverAccount, UserWallet senderWallet, UserWallet receiverWallet, decimal amount)
        {

            if (sourceAccount == null || receiverAccount == null || senderWallet == null || receiverWallet == null)
                return false;

            var senederWalletPreviousBalance = senderWallet.CurrentBalance;
            senderWallet.PrevioseBalance = senederWalletPreviousBalance;
            senderWallet.CurrentBalance -= amount;

            var sourcePreviousAccount = sourceAccount.CurrentBalance;
            sourceAccount.PrevioseBalance = sourcePreviousAccount;
            sourceAccount.CurrentBalance += amount;

            var receiverWalletPreviousBalance = receiverWallet.CurrentBalance;
            receiverWallet.PrevioseBalance = receiverWalletPreviousBalance;
            receiverWallet.CurrentBalance += amount;

            var receiverAccountPreviousBalance = receiverAccount.CurrentBalance;
            receiverAccount.PrevioseBalance += receiverAccountPreviousBalance;
            receiverAccount.CurrentBalance -= amount;

           /* sourceAccount.LegerBalance-= amount;    
            receiverAccount.LegerBalance += amount;*/

            _appContext.UserWallets.UpdateRange(new List<UserWallet> { senderWallet, receiverWallet });
           _appContext.Accounts.UpdateRange(new List<Models.DTO.Account> { sourceAccount, receiverAccount });
            return await _appContext.SaveChangesAsync() > 0;
        }

        public async Task<ApiBaseResponse<OfflineSettlementResponseDTO>> ProcessOfflineSettlement(OfflinePaymentSettlementRequestDTO requestModel, string userId)
        {
            try
            {
                //Remove existing transaction
                _logger.LogInformation("Started Processing here");
                var unSettledTransactions = await RemoveSettledTransactions(requestModel.OfflineTransactions);
                // do settllement
                var setlementResponse = await SettleTransactions(unSettledTransactions);

                byte[] key = new byte[16];

                byte[] iv = new byte[16];

                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(key);
                    rng.GetBytes(iv);
                }

                var userOfflineWallets = await GetListOfUserOflineWallet(userId);

                var serialisedObject = string.Empty;

                if (userOfflineWallets.Count > 0)
                {
                    serialisedObject = SecurityHelper.SerializeObject<List<UserWallet>>(userOfflineWallets);
                }

                var encryptedWallets = SecurityHelper.Encrypt(serialisedObject, key, iv);

                var response = new OfflineSettlementResponseDTO
                {
                    TotalSettledTransaction = unSettledTransactions.Count == 0 ? 0 : requestModel.OfflineTransactions.Count - setlementResponse.UnSettledTransaction.Count,
                    TotalUnsettledTransaction = unSettledTransactions.Count == 0 ? requestModel.OfflineTransactions.Count : setlementResponse.UnSettledTransaction.Count,
                    UnsettledTransactions = setlementResponse.UnSettledTransaction,
                    NewBalance = setlementResponse.Balance,
                    EncryptedWallets = encryptedWallets,
                    key = key,
                    Signature = iv
                };


                return new ApiBaseResponse<OfflineSettlementResponseDTO>
                {
                    Data = response,
                    Errors = null,
                    IsSuccessful= true,
                    Message = "Transactions were settled successfully",
                    ResponseCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<UserWallet>> GetListOfUserOflineWallet(string userId)
        {
                   return  _appContext.UserWallets.Include(x => x.WalletType)
                     .Where(x => x.WalletType.Name.ToLower() == Data.Enum.WalletType.OFFLINE.ToString().ToLower() && x.UserUid.ToString() == userId)
                     .ToList();
        }
         
    }
}
