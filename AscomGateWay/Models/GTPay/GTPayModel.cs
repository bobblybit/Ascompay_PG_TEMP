using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.GTPay
{

    public class GTPayRequest
    {
        public double amount { get; set; }
        public string email { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string initiate_type { get; set; } = "inline";
        public string transaction_ref { get; set; } = string.Empty;
        public string callback_url { get; set; } = string.Empty;

    }

    public class GTPayResponse
    {
        public int status { get; set; }
        public string message { get; set; } = string.Empty;
        public GTPayResponseData data { get; set; } = new GTPayResponseData();

    }
    public class GTPayResponseData
    {
        public string checkout_url { get; set; } = string.Empty;

    }


    public class GTPayQueryResponse
    {
        public int status { get; set; }
        public string message { get; set; } = string.Empty;
        public string success { get; set; } = string.Empty;
        public GTPayQueryResponseData data { get; set; } = new GTPayQueryResponseData();

    }

    public class GTPayQueryResponseData
    {
        public string transaction_status { get; set; } = string.Empty;
        public string transaction_type { get; set; } = string.Empty;
        public string gateway_transaction_ref { get; set; } = string.Empty;

    }


    public enum GTPayTransactionstatusEnum
    {
        Success
    }

    public class GTPayViewModel
    {
        public string checkout_url { get; set; } = string.Empty;
    }
    public class FundTransfer
    {
        public string? transaction_reference { get; set; }
        [Required]
        public string amount { get; set; }
        [Required]
        public string bank_code { get; set; }
        [Required]
        public string account_number { get; set; }
        [Required]
        public string account_name { get; set; }
        [Required]
        public string currency_id { get; set; }
        public string remark { get; set; }
    }
}