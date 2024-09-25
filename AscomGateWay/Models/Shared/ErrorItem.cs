using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AscomPayPG.Models.Shared
{
    public class ErrorItem
    {
        public string Key { get; set; }

        public List<string> ErrorMessages { get; set; }
    }
}
