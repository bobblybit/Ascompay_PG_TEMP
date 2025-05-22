namespace AscomPayPG.Helpers.HTTPHelper.UrlHelper
{
    public static class NinePSBVatUrls
    {
      /* // private static readonly string baseUrl = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVasConfiuration:BaseUrl");
        private static readonly string apiVersion = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVasConfiuration:ApiVersion");
        
        public static string Authenticate = $"{baseUrl}identity/{apiVersion}authenticate";
        public static string PhoneNetworks = $"{baseUrl}vas/{apiVersion}topup/network?phone=[phoneNumber]";
        public static string AirTimePurchase = $"{baseUrl}vas/{apiVersion}topup/airtime";
        public static string DataPlans = $"{baseUrl}vas/{apiVersion}topup/dataPlans?phone=[phoneNumber]";
        public static string DataPurchase = $"{baseUrl}vas/{apiVersion}topup/data";
        public static string TransactionStatus = $"{baseUrl}vas/{apiVersion}topup/status?transReference=[transReference]";
        public static string BillCategories = $"{baseUrl}vas/{apiVersion}billspayment/categories";
        public static string CategoryBiller = $"{baseUrl}vas/{apiVersion}billspayment/billers/[categoryId]";
        public static string BillerInputFields = $"{baseUrl}vas/{apiVersion}billspayment/fields/[billerId]";
        public static string BillerInputValidate = $"{baseUrl}vas/{apiVersion}billspayment/validate";
        public static string BillPayment = $"{baseUrl}vas/{apiVersion}billspayment/pay";
        public static string BillPaymentStatus = $"{baseUrl}vas/{apiVersion}billspayment/status?transReference=[transReference]";*/
        #region NEW URLS FOR PAYMENTGATEWAY 
           // private static readonly string PGM_apiVersion = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVasConfiuration:ApiVersion");
            private static readonly string PGM_baseUrl = ConfigurationHelper.GetAppSettingSectionSingleProperty("9PSBVasConfiuration:BaseUrl");
            public static string PGM_Authenticate = $"{PGM_baseUrl}api/identity/authenticate";
            public static string PGM_PhoneNetworks = $"{PGM_baseUrl}api/vas/phone-network?phone=[phoneNumber]";
            public static string PGM_DataPlans = $"{PGM_baseUrl}api/vas/data-plans?phoneNumber=[phoneNumber]";
            public static string PGM_TransactionStatus = $"{PGM_baseUrl}api/vas/transaction-status/[transReference]";
            public static string PGM_AirTimePurchase = $"{PGM_baseUrl}api/vas/airtime-top-up";
            public static string PGM_DataPurchase = $"{PGM_baseUrl}api/vas/data-top-up";
            public static string PGM_BillCategories = $"{PGM_baseUrl}api/vas/categories";
            public static string PGM_BillPaymentStatus = $"{PGM_baseUrl}api/vas/biller-payment-status?transactionReference=[transReference]";
            public static string PGM_CategoryBiller = $"{PGM_baseUrl}api/vas/category-biller/[categoryId]";
            public static string PGM_BillerInputFields = $"{PGM_baseUrl}api/vas/biller-input-field/[billerId]";
            public static string PGM_BillerInputValidate = $"{PGM_baseUrl}api/vas/biller-input-validation";
            public static string PGM_BillPayment = $"{PGM_baseUrl}api/vas/biller-payment";
        #endregion

    }
}
