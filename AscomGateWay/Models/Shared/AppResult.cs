namespace AscomPayPG.Models.Shared
{
    public class AppResult<T>
    {
        public AppResult()
        {
            Data = new List<T>();
            Status = false;
            Message = string.Empty;
        }
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; }
    }
}