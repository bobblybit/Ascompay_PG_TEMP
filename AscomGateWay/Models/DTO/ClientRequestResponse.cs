namespace AscomPayPG.Models.DTO
{
    public class ClientRequestResponse
    {
        public ClientRequestResponse()
        {
            Page = 1;
            Transactions = new List<ClientRequest>();
        }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<ClientRequest> Transactions { get; set; }


    }
}