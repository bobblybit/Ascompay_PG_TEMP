using System.Security.Cryptography;
using AscomPayPG.OfflineSecurity.DTO;
using AscomPayPG.OfflineSecurity.Models;
using AscomPayPG.OfflineSecurity.Service.Interfaces;
using AscomPayPG.Services.Interface;
using Microsoft.Data.SqlClient;

namespace AscomPayPG.OfflineSecurity.Service.Implementation
{
    public class TransactionProcessor
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletService _walletService;
        private readonly ITransactionValidator _validator;
        private readonly ITransactionValidator _transactionValidator;  // Added for hash validation
        private readonly ILogger<TransactionProcessor> _logger;
        private readonly string _connectionString;

        public TransactionProcessor(ITransactionRepository transactionRepository,
                                    ITransactionValidator validator,
                                     string connectionString,
                                     IWalletService walletService,
                                     ITransactionValidator transactionValidator,  // Injected the hash validator
                                     ILogger<TransactionProcessor> logger)
        {
            _transactionRepository = transactionRepository;
            _walletService = walletService;
            _validator = validator;
            _connectionString = connectionString;
            _transactionValidator = transactionValidator;
            _logger = logger;
        }

        // Method to process a list of offline transactions
        public async Task<ProcessingResult> ProcessTransactionRequest(OfflineTransactionRequest request)
        {
            var results = new List<TransactionResult>();
            var processingErrors = new List<string>();
            // Process each transaction in the request
            var batchisValid = await _validator.ValidateBatchAndTransactions(request);
            if (batchisValid)
            {

                foreach (var transaction in request.OfflineTransactions)
                {
                    bool result = false;
                    try
                    {
                        // Validate the transaction hash
                        bool isValid = await _validator.ValidateTransaction(transaction);

                        if (isValid)
                        {
                            // Process individual transaction
                            result = await ProcessTransaction(transaction);

                            // Add the result with the appropriate message
                            results.Add(new TransactionResult
                            {
                                TransactionId = transaction.TransactionId,
                                IsProcessed = result,
                                ResponseMessages = result ? null : "Failed to process transaction."
                            });
                        }
                        else
                        {
                            results.Add(new TransactionResult
                            {
                                IsProcessed = result,
                                TransactionId = transaction.TransactionId,
                                ResponseMessages = result ? null : "Invalid transaction hash."
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                        // Log and store error details for this transaction
                        processingErrors.Add($"Transaction {transaction.TransactionId} failed due to an exception: {ex.Message}");

                        results.Add(new TransactionResult
                        {
                            TransactionId = transaction.TransactionId,
                            IsProcessed = false,
                            ResponseMessages = $"Error processing transaction: {ex.Message}"
                        });
                    }
                }

            }
            else
            {
                results.Add(new TransactionResult
                {
                    IsProcessed = false,
                    TransactionId = null,
                    ResponseMessages = "batch-request is compromised"
                });
            }


            // Check if any transaction failed and return a summary
            bool allProcessedSuccessfully = results.All(r => r.IsProcessed);

            //generate new ounce

            GenerateAndSaveNonce(request.keys.userUid, request.keys.nonce);

            return new ProcessingResult
            {
                IsSuccessful = allProcessedSuccessfully,
                ProcessedTransactions = results,
                ResponseMessages = processingErrors.Any() ? processingErrors : null
            };
        }

        // Method to process each transaction individually
        private async Task<bool> ProcessTransaction(OfflineTransactionSec transaction)
        {
            try
            {
                // 2. Validate transaction details (amount, wallets, etc.)
                if (!await ValidateTransactionDetails(transaction))
                {
                    _logger.LogWarning($"Transaction {transaction.TransactionId} validation failed.");
                    return false;
                }

                // 3. Update sender and receiver wallets
                if (!await UpdateWallets(transaction))
                {
                    _logger.LogWarning($"Transaction {transaction.TransactionId} failed to update wallets.");
                    return false;
                }

                // 4. Record the transaction in the database
                await _transactionRepository.SaveTransaction(transaction);

                // 5. Mark transaction as completed
                transaction.TransactionStatus = true;
                await _transactionRepository.UpdateTransactionStatus(transaction.TransactionId, true);

                _logger.LogInformation($"Transaction {transaction.TransactionId} processed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing transaction {transaction.TransactionId}");
                return false;
            }
        }

        // Method to validate the details of a transaction
        private async Task<bool> ValidateTransactionDetails(OfflineTransactionSec transaction)
        {
            // Ensure sender and receiver wallet IDs are valid
            if (string.IsNullOrEmpty(transaction.SenderWalletId) || string.IsNullOrEmpty(transaction.ReceiverWalletId))
            {
                _logger.LogWarning("Sender or Receiver WalletId is invalid.");
                return false;
            }

            // Additional validations based on your business logic
            // e.g., check if the sender has enough balance, etc.
            return true;
        }

        // Method to update sender and receiver wallet balances
        private async Task<bool> UpdateWallets(OfflineTransactionSec transaction)
        {
            // Get the sender and receiver wallets
            var senderWallet = await _walletService.GetIntenalWalletById(transaction.SenderWalletId);
            var receiverWallet = await _walletService.GetIntenalWalletById(transaction.ReceiverWalletId);

            if (senderWallet == null || receiverWallet == null)
            {
                _logger.LogWarning("Sender or Receiver wallet does not exist.");
                return false;
            }

            // Check if the sender has enough balance
            if (senderWallet.CurrentBalance < transaction.TransactionAmount)
            {
                _logger.LogWarning("Insufficient balance in sender's wallet.");
                return false;
            }

            // Deduct the amount from sender's wallet and add it to the receiver's wallet
            senderWallet.CurrentBalance -= transaction.TransactionAmount;
            receiverWallet.CurrentBalance += transaction.TransactionAmount;

            // Update the wallet balances
            await _walletService.UpdateIntenalWallet(senderWallet);
            await _walletService.UpdateIntenalWallet(receiverWallet);

            return true;
        }


        public string GenerateAndSaveNonce(Guid userUid, string mobileAppPublicId)
        {
            // Step 1: Generate the nonce
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            byte[] randomBytes = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            string randomString = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
            string nonce = $"{timestamp}-{randomString}";

            // Step 2: Save the nonce to the database
            SaveNonceToDatabase(userUid, mobileAppPublicId, nonce);

            // Return the generated nonce
            return nonce;
        }

        private void SaveNonceToDatabase(Guid userUid, string mobileAppPublicId, string nonce)
        {
            const string query = @"
            INSERT INTO NonceTable (UserUid, MobileAppPublicId, Nonce, CreatedAt)
            VALUES (@UserUid, @MobileAppPublicId, @Nonce, @CreatedAt)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserUid", userUid);
                    command.Parameters.AddWithValue("@MobileAppPublicId", mobileAppPublicId);
                    command.Parameters.AddWithValue("@Nonce", nonce);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                    command.ExecuteNonQuery();
                }
            }
        }
    }


}
