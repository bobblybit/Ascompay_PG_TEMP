using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AscomPayPG.Models.Shared
{
    public class ApiBaseResponse<T> where T : new()
    {
        public int ResponseCode { get; set; }

        public bool IsSuccessful { get; set; }

        public string Message { get; set; }

        public List<ErrorItem> Errors { get; set; }

        public T Data { get; set; }

        public ApiBaseResponse()
        {
            Errors = new List<ErrorItem>();
        }
    }
}
