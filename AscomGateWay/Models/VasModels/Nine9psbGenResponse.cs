namespace PaymentGateWayMiddleWare.Model
{
    public class Nine9psbGenResponse<T>
    {
        public string Message { get; set; }
        public  bool IsSuccessful { get; set; }
        public string status { get; set; }
        public int ResponseCode { get; set; }
        public T Data { get; set; }

    }

    public class Nine9psbListGenResponse<T>
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public string status { get; set; }
        public int ResponseCode { get; set; }
        public List<T> Data { get; set; }

    }
}
