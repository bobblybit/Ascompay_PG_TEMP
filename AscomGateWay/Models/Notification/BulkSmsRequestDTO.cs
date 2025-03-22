namespace AscomPayPG.Models.Notification
{
    public class BulkSmsRequestDTO
    {
        public string body { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string api_token { get; set; }
        public string gateway { get; set; }
        public string customer_reference { get; set; }
        public string callback_url { get; set; }
    }

    public class BulkSmsResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public string message_id { get; set; }
        public double cost { get; set; }
        public string currency { get; set; }
        public string gateway_used { get; set; }
    }
    public class BulkSmsResponseData
    {
        public BulkSmsResponse data { get; set; }
    }
}
