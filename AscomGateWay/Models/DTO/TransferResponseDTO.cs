namespace AscomPayPG.Models.DTO
{
    public class TransferResponseDTO
    {
        public string Message { get; set; }
        public bool IsSuccessfull { get; set; }
        public decimal SendNewBalance { get; set; }
        public int TransferStatus { get; set; }
    }
}
