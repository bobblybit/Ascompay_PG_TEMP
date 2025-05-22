namespace AscomPayPG.Models.Shared
{
    public class PlainResponse
    {
        public bool IsSuccessful { get; set; }
        public string  transaction_reference { get; set; }
        public string Message { get; set; }
        public int ResponseCode { get; set; }
        public dynamic Data { get; set; }
    }
}
