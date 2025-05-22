namespace AscomPayPG.Models.VasModels
{
    public class VatAuthResponse
    {
        public bool IsSuccessfull { get; set; }
        public string Token { get; set; }
    }
    public class PhoneNumberLookUpResponse
    {
        public string Network { get; set; }
    }

    public class AirtimeTopUp
    {
        public string Network { get; set; }
        public string Recipient { get; set; }
        public string Amount { get; set; }
    }

    public class DataTopUpResponse
    {
        public string Network { get; set; }
        public string Recipient { get; set; }
        public string Amount { get; set; }
        public string DataPlan { get; set; }
    }

    public class StatusResponse
    {
        public string TransactionStatus { get; set; }
        public string Description { get; set; }
    }

    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class CategoryBiller
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class BillerField
    {
        public string Validation { get; set; }
        public string FieldDescription { get; set; }
        public string FieldName { get; set; }
        public string IsSelectData { get; set; }
        public List<SelectItem> Items { get; set; }
    }

    public class SelectItem
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
    }


    public class BillerFieldValidationResponse
    {
        public string CustomerName { get; set; }
        public string Amount { get; set; }
        public string OtherField { get; set; }
    }

    public class BillerPaymentResponse
    {
        public string OtherField { get; set; }
        public bool IsToken { get; set; }
        public string Token { get; set; }
    }

    public class DataPlansResponse
    {
        public string productId { get; set; }
        public string dataBundle { get; set; }
        public string amount { get; set; }
        public string validity { get; set; }
    }


    public class TransactionStatusResponse
    {
        public string TransactionStatus { get; set; }
        public string Description { get; set; }
    }
}
