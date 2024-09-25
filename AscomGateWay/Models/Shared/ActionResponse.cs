using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AscomPayPG.Models.Shared
{
    public class ActionResponse<T> where T : new()
    {
        public ActionResponse()
        {
            Status = false;
            Data = new T();
            Message = string.Empty;
        }
        public bool Status { get; set; }
        public long Id { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
