namespace AscomPayPG.Models.DTO
{
    public class VirtualAccount_VM
    {
        public string transaction_reference { get; set; }
        public string virtual_account_number { get; set; }
        public string principal_amount { get; set; }
        public string settled_amount { get; set; }
        public string fee_charged { get; set; }
        public string transaction_date { get; set; }
        public string customer_identifier { get; set; }
        public string transaction_indicator { get; set; }
        public string remarks { get; set; }
        public string currency { get; set; }
        public string channel { get; set; }
        public MetaBody_VM meta { get; set; }
        public string encrypted_body { get; set; }
    }
    public class MetaBody_VM
    {
        public string? freeze_transaction_ref { get; set; }
        public string? reason_for_frozen_transaction { get; set; }
    }
}
