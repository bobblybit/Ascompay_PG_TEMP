namespace AscomPayPG.OfflineSecurity.DTO
{
    public class TransactionResult
    {
        public string TransactionId { get; set; }
        public bool IsProcessed { get; set; }
        public string ResponseMessages { get; set; }
    }
}
