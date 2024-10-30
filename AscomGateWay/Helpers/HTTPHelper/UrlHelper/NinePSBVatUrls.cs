namespace AscomPayPG.Helpers.HTTPHelper.UrlHelper
{
    public static class NinePSBVatUrls
    {
        private static readonly string baseUrl = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVatConfiuration:BaseUrl");
        private static readonly string apiVersion = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVatConfiuration:ApiVersion");

        public static string Authenticate = $"{baseUrl}identity/{apiVersion}authenticate";
        public static string PhoneNetworks = $"{baseUrl}vas/{apiVersion}network?phone=[phoneNumber]";
        public static string AirTimePurchase = $"{baseUrl}vas/{apiVersion}topup/airtime";
        public static string DataPlans = $"{baseUrl}vas/{apiVersion}topup/dataplans?phone=[phoneNumber]";
        public static string DataPurchase = $"{baseUrl}vas/{apiVersion}topup/data";
        public static string TransactionStatus = $"{baseUrl}vas/{apiVersion}status?transReference=[transReference]";
        public static string BillCategories = $"{baseUrl}vas/{apiVersion}/billspayment/categories";
        public static string CategoryBiller = $"{baseUrl}vas/{apiVersion}/billspayment/billers/[categoryId]";
        public static string BillerInputFields = $"{baseUrl}vas/{apiVersion}/billspayment/fields/[billerId]";
        public static string BillerInputValidate = $"{baseUrl}vas/{apiVersion}/billspayment/validate";
        public static string BillPayment = $"{baseUrl}vas/{apiVersion}/billspayment/pay";
        public static string BillPaymentStatus = $"{baseUrl}vas/{apiVersion}/billspayment/status?transReference=[transReference]";
    }
}
