using AscomPayPG.Data;
using AscomPayPG.Models.DTO;
using AscomPayPG.Services.Interface;

namespace AscomPayPG.Helpers
{
    public class TransactionHelper : ITransactionHelper
    {
        private readonly AppDbContext _appContext;
        private static List<TransactionType> _transactionTypes;

        public TransactionHelper(AppDbContext appDbContext)
        {
            _appContext= appDbContext;
        }

        public static decimal CalculateVAT(decimal amount) => Math.Round((decimal)0.75 / 100 * amount, 1);
       
        public async Task<decimal> CalculateCharges(decimal amount, string transactionType)
        {
            var transactionTypeDetails = _appContext.TransactionType.FirstOrDefault( x => x.Ttype
                                                                                      .Replace(" ", "")
                                                                                      .Replace("(","")
                                                                                      .Replace(")","") == transactionType);
            if (transactionTypeDetails != null)
            {

             if (transactionTypeDetails.By_Percent)
                return Math.Round((decimal)transactionTypeDetails.T_Percentage / 100 * amount, 1);
             else
                return transactionTypeDetails.T_Amount;
            }
            return 0;
        }
    }
}
