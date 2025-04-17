namespace AscomPayPG.Models
{
    public class AccountLookUpLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string InitaitorId { get; set; }
        public string LookUpId { get; set; }
        public string LookUpBank { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public bool LookStatus { get; set; }
        public int UsageStatus { get; set; }
        public DateTime DateCreated { get; set; }  = DateTime.Now;
        public DateTime LastCreated { get; set; } = DateTime.Now;
        public AccountLookUpLog() { }   
    }
}
