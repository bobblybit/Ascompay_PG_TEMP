namespace AscomPayPG.Models.Shared
{

    public class PaymentResponse
    {
        public bool Status { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
    public class PayQueryResponse
    {
        public bool Status { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class PaymentResponseView
    {
        public PaymentResponseView()
        {
            Data = new object();
            Reference = string.Empty;
            Customer = string.Empty;
            Gateway = string.Empty;
            Message = string.Empty;
            RequestingClientUrl = string.Empty;

        }
        public bool Status { get; set; }
        public string Message { get; set; }

        public string Reference { get; set; }
        public string Customer { get; set; }
        public decimal TransactionsAmount { get; set; }
        public string Gateway { get; set; }
        public string RequestingClientUrl { get; set; }
        public object Data { get; set; }
    }


    public class PayResponse
    {
        public PayResponse()
        {
            Message = string.Empty;
            RedirectUrl = string.Empty;
        }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}