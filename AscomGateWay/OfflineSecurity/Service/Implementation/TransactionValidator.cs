using AscomPayPG.OfflineSecurity.Models;
using AscomPayPG.OfflineSecurity.Service.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace AscomPayPG.OfflineSecurity.Service.Implementation;

public class TransactionValidator : ITransactionValidator
{
    private readonly ILogger<TransactionValidator> _logger;
    private readonly ITransactionRepository _transactionRepository;
    private readonly RSA _rsa;

    public TransactionValidator(ILogger<TransactionValidator> logger, ITransactionRepository transactionRepository)
    {
        _logger = logger;
        // Initialize the RSA for public/private key encryption
        _rsa = RSA.Create();
        _transactionRepository = transactionRepository;
    }

    public async Task<bool> CheckNonce(string nonce)
    {
        // Check if the nonce already exists in the database or cache
        bool isNonceUsed = await _transactionRepository.IsNonceUsed(nonce);

        if (isNonceUsed)
        {
            // Log and return false if the nonce has already been used (replay attack)
            _logger.LogWarning($"Nonce {nonce} has already been used for a previous transaction.");
            return false;
        }

        // Mark the nonce as used by saving it to the database (or cache)
        await _transactionRepository.SaveNonce(nonce);

        // Return true if the nonce is valid and unique
        return true;

    }

    // Method to log the details of the transaction
    private void LogTransactionDetails(OfflineTransactionSec transaction)
    {
        _logger.LogInformation($"Transaction {transaction.TransactionId} Details:");
        _logger.LogInformation($"- Sender Wallet ID: {transaction.SenderWalletId}");
        _logger.LogInformation($"- Receiver Wallet ID: {transaction.ReceiverWalletId}");
        _logger.LogInformation($"- Transaction Type: {transaction.TransactionType}");
        _logger.LogInformation($"- TransactionAmount: {transaction.TransactionAmount}");
        _logger.LogInformation($"- Transaction Date: {transaction.TransactionDate:yyyy-MM-dd HH:mm:ss}");
    }

    // Method to validate the hash of the transaction
    public async Task<bool> ValidateTransaction(OfflineTransactionSec transactionsRequest)
    {
        // Step 1: Verify the signature with the public key
        bool isSignatureValid = VerifyTransactionHash(transactionsRequest);
        if (!isSignatureValid)
        {
            _logger.LogWarning($"batch verification failed.");
            return false;
        }

        // Step 4: Log transaction details
        LogTransactionDetails(transactionsRequest);
        return true;
    }

    // Helper method to verify the transaction signature
    public bool VerifyTransactionHash(OfflineTransactionSec transaction)
    {
        var sendersUid = GetUserUidBySenderWalletId(transaction.SenderWalletId);
        // Recreate the transaction hash

        //TODO: Get user nonce

        string transactionDataToHash = $"{transaction.TransactionId}|{sendersUid}|{transaction.SenderWalletId}|{transaction.ReceiverWalletId}|{transaction.TransactionAmount}|{transaction.TransactionDate}";

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] transactionDataBytes = Encoding.UTF8.GetBytes(transactionDataToHash);
            byte[] recreatedTransactionHashBytes = sha256.ComputeHash(transactionDataBytes);
            string recreatedTransactionHash = BitConverter.ToString(recreatedTransactionHashBytes).Replace("-", "").ToLower();

            // Compare the recreated transaction hash with the received hash
            if (!recreatedTransactionHash.Equals(transaction.Hash, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Transaction hash mismatch for Transaction ID: {transaction.TransactionId}");
                _logger.LogWarning($"Transaction hash validation failed for TransactionId: {transaction.TransactionId}");
                return false;
            }
        }

        return true;
    }

    private RSA GetUserPublicKeyFromDatabase(Guid userUid)
    {
        // Simulate database retrieval
        var UserPlublicID = "TESTING123455667TostAscom";

        RSA publicKey = RSA.Create();
        publicKey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(UserPlublicID), out _);
        return publicKey;
    }


    // Simulating the database lookup for UserUid by SenderWalletId
    public string GetUserUidBySenderWalletId(string senderWalletId)
    {
        // Simulating a database of wallets and their associated UserUids
        var walletDatabase = new Dictionary<string, string>
            {
                { "wallet123", "user1" },
                { "wallet456", "user2" },
                { "wallet789", "user3" }
            };

        // Lookup the sender's wallet ID in the simulated database
        if (walletDatabase.ContainsKey(senderWalletId))
        {
            return walletDatabase[senderWalletId]; // Return the corresponding UserUid
        }
        else
        {
            Console.WriteLine("Sender Wallet ID not found.");
            return null;
        }
    }

    public async Task<bool> ValidateBatchAndTransactions(OfflineTransactionRequest transactionsRequest)
    {
        // Step 0.1: Fetch the user's public key using userUid
        RSA thepublicKey = GetUserPublicKeyFromDatabase(transactionsRequest.keys.userUid);
        if (thepublicKey == null)
        {
            Console.WriteLine("User's public key not found. Validation cannot proceed.");
            return false;
        }


        // Step 2: Check the nonce for uniqueness
        bool isNonceUnique = await CheckNonce(transactionsRequest.keys.nonce);
        if (!isNonceUnique)
        {
            _logger.LogWarning($"This batch has a replayed nonce.");
            return false;
        }

        // Step 1: Recreate the batch hash
        StringBuilder batchDataBuilder = new StringBuilder();
        foreach (var transaction in transactionsRequest.OfflineTransactions)
        {
            string transactionData = $"{transaction.TransactionId}|{transactionsRequest.keys.userUid}|{transaction.SenderWalletId}|{transaction.ReceiverWalletId}|{transaction.TransactionAmount}|{transaction.TransactionDate}|{transactionsRequest.keys.nonce}";
            batchDataBuilder.Append(transactionData);
        }

        string recreatedBatchData = batchDataBuilder.ToString();

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] recreatedBatchDataBytes = Encoding.UTF8.GetBytes(recreatedBatchData);
            byte[] recreatedBatchHashBytes = sha256.ComputeHash(recreatedBatchDataBytes);
            string recreatedBatchHash = BitConverter.ToString(recreatedBatchHashBytes).Replace("-", "").ToLower();

            // Step 2: Compare the recreated batch hash with the received batch hash
            if (!recreatedBatchHash.Equals(transactionsRequest.keys.BatchHash, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Batch hash mismatch. Data integrity compromised.");
                return false;
            }

            // Step 3: Verify the batch signature using the public key
            byte[] batchHashBytes = Encoding.UTF8.GetBytes(transactionsRequest.keys.BatchHash);
            byte[] receivedBatchSignatureBytes = Convert.FromBase64String(transactionsRequest.keys.BatchSignature);

            bool isBatchSignatureValid = thepublicKey.VerifyData(
                recreatedBatchHashBytes,
                receivedBatchSignatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            if (!isBatchSignatureValid)
            {
                Console.WriteLine("Batch signature verification failed.");
                return false;
            }
        }

        // If all checks pass
        return true;
    }

}