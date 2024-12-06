namespace AscomPayPG.OfflineSecurity.DTO
{
    public class ProcessingResult
    {
        public bool IsSuccessful { get; set; }
        public List<TransactionResult> ProcessedTransactions { get; set; }
        public string new_Ounce { get; set; }
        public List<string> ResponseMessages { get; set; }
    }
}
