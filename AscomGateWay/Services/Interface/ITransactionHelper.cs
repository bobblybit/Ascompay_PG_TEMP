using AscomPayPG.Models.DTO;

namespace AscomPayPG.Services.Interface
{
    public interface ITransactionHelper
    {
        public Task<decimal> CalculateCharges(decimal amount, string transactionType);
        public Task<decimal> CalculateVAT(decimal amount, string transactionType);
        public Task<decimal> GetPaymentProviderCharges(string transactionType);
        public Task NotifyForCredit(string receiverFullName, string receiverEmail, string sender, string amount, string balance, string transactionTime, string decription);
        public void NotifyForDebit(string mailReciever, string receiverFullName, string amount, string balance, string vat, string charges, string transactionTime, string description, string transactionReference);
        public Task NotifyForDebitSMS(User appUser, string sourceAccount, string amount, string balance, string description);
        public Task NotifyForCreditSMS(User appUser, string receiverAccount, string amount, string balance, string description);
    }
}
