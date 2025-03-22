namespace AscomPayPG.Models.DTO;

public class TransactionVerificationResponseDTO
{
        public bool IsOk { get; set; }
}
public class TransactionVerificationRequestDTO
{
    public string SourceAccount { get; set; }
    public string ReceiverAccount { get; set; }
    public string Amount { get; set; }
    public string Token { get; set; }
    public string TransactionType { get; set; }
}
