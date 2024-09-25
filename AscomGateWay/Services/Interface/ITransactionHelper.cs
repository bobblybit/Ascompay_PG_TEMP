namespace AscomPayPG.Services.Interface
{
    public interface ITransactionHelper
    {
        public Task<decimal> CalculateCharges(decimal amount, string transactionType);
    }
}
