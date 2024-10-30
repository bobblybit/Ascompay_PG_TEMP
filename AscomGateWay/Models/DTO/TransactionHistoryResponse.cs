namespace AscomPayPG.Models.DTO
{
    public class TransactionsResponse
    {
        public TransactionsResponse()
        {
            Page = 1;
            Transactions = new List<Transactions>();
        }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<Transactions> Transactions { get; set; }


    }
}