namespace AscomPayPG.Models.Shared
{

    public class QueryRequest
    {
        /*
             * gtpay_tranx_id,
             * gtpay_tranx_amt,
             * gtpay_tranx_curr,
             * gtpay_cust_id,
        */
        public string TransactionId { get; set; } = string.Empty;
    }
}