namespace AscomPayPG.Models.Paystack
{
   public class PaystackPayRequest
    {
        public string email { get; set; } = string.Empty;
        public string amount { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public string currency { get; set; } = "NGN";
        public string callback { get; set; } = string.Empty;

    }

    public class PaystackPayResponse
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public PaystackPayResponseData data { get; set; } = new PaystackPayResponseData();

    }
    public class PaystackPaymentResponse
    {
        public string Data { get; set; } = string.Empty;

    }
    public class PaystackPayResponseData
    {
        public string authorization_url { get; set; } = string.Empty;
        public string access_code { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;

    }


    public class PaystackPayQueryResponse
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public PaystackPayQueryResponseData data { get; set; } = new PaystackPayQueryResponseData();

    }

    public class PaystackPayQueryResponseData
    {
        public string amount { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string transaction_date { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;

    }
}