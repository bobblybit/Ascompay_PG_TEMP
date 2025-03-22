namespace AscomPayPG.Models.DTO
{
    public class MyKudiSmsResponseDTO
    {
        public string status { get; set; }
        public string error_code { get; set; }
        public string cost { get; set; }
        public string data { get; set; }
        public string msg { get; set; }
        public int length { get; set; }
        public int page { get; set; }
        public string balance { get; set; }
    }
}
