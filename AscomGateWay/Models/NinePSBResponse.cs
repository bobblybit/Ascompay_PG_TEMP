namespace DB_MODALS.DTO
{
    public class NinePSBResponse
    {
        public bool IsSuccessful { get; set; }
        public string transaction_reference { get; set; }
        public string Message { get; set; }
        public int ResponseCode { get; set; }
        public WalletEnquiryResponse Data { get; set; }
        public string Status { get; set; }
    }

    public class WalletEnquiryResponse
    {
        public string productCode { get; set; }
        public string responseDescription { get; set; }
        public string lienStatus { get; set; }
        public string bvn { get; set; }
        public decimal availableBalance { get; set; }
        public string freezeStatus { get; set; }
        public decimal ledgerBalance { get; set; }
        public decimal maximumBalance { get; set; }
        public string nuban { get; set; }
        public string number { get; set; }
        public string phoneNo { get; set; }
        public string phoneNuber { get; set; }
        public string pndstatus { get; set; }
        public string tier { get; set; }
        public string responseCode { get; set; }
        public bool isSuccessful { get; set; }
        public string status { get; set; }
        public string name { get; set; }
    }
}
