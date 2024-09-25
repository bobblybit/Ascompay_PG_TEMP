
using AscomPayPG.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscomPayPG.Models
{
    public class WalletAgentModel : BaseEntity
    {
        public string WalletUid { get; set; }
        public string? UserUid { get; set; }
        //public int ShareOfPayment { get; set; }
        public string AgentAccountName { get; set; }
        public string AgentEmail { get; set; }
        public string AgentPhoneNumber { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string Status { get; set; }
        public string? TotalTransactionAmount { get; set; }
    }
}
