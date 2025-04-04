﻿using AscomPayPG.OfflineSecurity.Models;

namespace AscomPayPG.OfflineSecurity.Service.Interfaces
{
    // Interface for Transaction Validator
    public interface ITransactionValidator
    {
        public Task<bool> ValidateTransaction(OfflineTransactionSec transactionRequest);
        public Task<bool> ValidateBatchAndTransactions(OfflineTransactionRequest transactionsRequest);
    }
}
