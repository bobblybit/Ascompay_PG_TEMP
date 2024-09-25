namespace AscomPayPG.Models
{
    public class Response<T>
    {
        public Response()
        {
            IsSuccess = false;
            Message = "";
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public dynamic Data { get; set; }
    }
}
