using Org.BouncyCastle.Tls;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.WAAS
{

    public class AuthenticateRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }

    }


    public class AuthenticateResponse
    {
        public string message { get; set; }
        public string accessToken { get; set; }
        public string expiresIn { get; set; }
        public string refreshToken { get; set; }
        public string refreshExpiresIn { get; set; }
        public string jwt { get; set; }
        public string tokenType { get; set; }

    }
    public class FieldErrors
    {
        public string eiusmode73 { get; set; }
    }

    public class GenericResponse
    {
        public string status { get; set; }
        public string responseCode { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
        public string error { get; set; }
        public FieldErrors fieldErrors { get; set; }
        public string jwt { get; set; }
    }
    public class OpenWalletRequest
    {
        public string bvn { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public string lastName { get; set; }
        public string otherNames { get; set; }
        public string phoneNo { get; set; }
        public string transactionTrackingRef { get; set; }
        public string accountName { get; set; }
        public string placeOfBirth { get; set; }
        public string address { get; set; }
        public string nationalIdentityNo { get; set; }
        public string nextOfKinPhoneNo { get; set; }
        public string nextOfKinName { get; set; }
        public string email { get; set; }
    }

    public class OpenWalletData
    {
        public string responseCode { get; set; }
        public string orderRef { get; set; }
        public string fullName { get; set; }
        public string creationMessage { get; set; }
        public string accountNumber { get; set; }
        public string ledgerBalance { get; set; }
        public string availableBalance { get; set; }
        public string customerID { get; set; }
        public string mfbcode { get; set; }
        public string financialDate { get; set; }
        public string withdrawableAmount { get; set; }
    }

    public class OpenWalletRequestFieldErrors
    {
        public string Ut1 { get; set; }
        public string occaecat_a47 { get; set; }
    }

    public class OpenWalletRequestResponse
    {
        public string status { get; set; }
        public string responseCode { get; set; }
        public string message { get; set; }
        public OpenWalletData data { get; set; }
        public string error { get; set; }
        public OpenWalletRequestFieldErrors fieldErrors { get; set; }
        public string jwt { get; set; }
    }

    public class WalletRequest
    {
        public string accountNo { get; set; }
    }

    public class GetWalletRequest
    {
        public string bvn { get; set; }
    }

    public class ChangeWalletStatusRequest
    {
        public string accountNumber { get; set; }
        public string accountStatus { get; set; }
    }


    public class WalletEnquiryData
    {
        public string name { get; set; }
        public string number { get; set; }
        public string responseCode { get; set; }
        public string requestStatus { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string phoneNo { get; set; }
        public string lastName { get; set; }
        public string tier { get; set; }
        public string productCode { get; set; }
        public string isSuccessful { get; set; }
        public string firstName { get; set; }
        public string maximumBalance { get; set; }
        public string responseMessage { get; set; }
        public string phoneNuber { get; set; }
        public string ledgerBalance { get; set; }
        public string nuban { get; set; }
        public string bvn { get; set; }
        public string responseDescription { get; set; }
        public string availableBalance { get; set; }
        public string maximumDeposit { get; set; }
        public string freezeStatus { get; set; }
        public string lienStatus { get; set; }
        public string pndstatus { get; set; }
        public string responseStatus { get; set; }
    }

    public class WalletEnquiryFieldErrors
    {
        public string mollit8 { get; set; }
    }

    public class WalletEnquiryResponse
    {
        public string status { get; set; }
        public string responseCode { get; set; }
        public string message { get; set; }
        public WalletEnquiryData data { get; set; }
        public string error { get; set; }
        public WalletEnquiryFieldErrors fieldErrors { get; set; }
        public string jwt { get; set; }
    }

    public class WalletStatusData
    {
        public string walletStatus { get; set; }
        public string responseCode { get; set; }
    }

    public class WalletStatusFieldErrors
    {
        public string labore49 { get; set; }
        public string qui_e { get; set; }
    }

    public class WalletStatusResponse
    {
        public string status { get; set; }
        public string responseCode { get; set; }
        public string message { get; set; }
        public WalletStatusData data { get; set; }
        public string error { get; set; }
        public FieldErrors fieldErrors { get; set; }
        public string jwt { get; set; }
    }

    public class WalletRequeryRequest
    {
        public string transactionId { get; set; }
        public string amount { get; set; }
        public string transactionType { get; set; }
        public string transactionDate { get; set; }
        public string accountNo { get; set; }
    }
    public class Merchant
    {
        public bool isFee { get; set; }
        public string merchantFeeAccount { get; set; }
        public string merchantFeeAmount { get; set; }
    }

    public class DebitWalletRequest
    {
        public string accountNo { get; set; }
        public string narration { get; set; }
        public decimal totalAmount { get; set; }
        public string transactionId { get; set; }
        public Merchant merchant { get; set; }
    }

    public class CreditWalletRequest
    {
        public string accountNo { get; set; }
        public string narration { get; set; }
        public double totalAmount { get; set; }
        public string transactionId { get; set; }
        public Merchant merchant { get; set; }
    }

    public class ChangeWalletStatusData
    {
        public string newWalletStatus { get; set; }
        public string responseCode { get; set; }
    }

    public class ChangeWalletStatusFieldErrors
    {
        public string commodoca { get; set; }
    }

    public class ChangeWalletStatusResponse
    {
        public string status { get; set; }
        public string responseCode { get; set; }
        public string message { get; set; }
        public ChangeWalletStatusData data { get; set; }
        public string error { get; set; }
        public FieldErrors fieldErrors { get; set; }
        public string jwt { get; set; }
    }
    public class WalletTransactionRequest
    {
        public string accountNumber { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string numberOfItems { get; set; }
    }

    public class OtherBankTransferDTO {
        public string bank { get; set; }
        public string senderAccountNumber { get; set; }
        /*public string senderName { get; set; }*/
        public string UserId { get; set; }
        public string Narration { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
    }

    public class OtherBankTransfer
    {
        public Customer customer { get; set; } = new Customer();
        public string narration { get; set; }
        public Order order { get; set; } = new Order();
        public Transaction transaction { get; set; } = new Transaction();
        public Merchant merchant { get; set; } = new Merchant();
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Customer
    {
        public Account account { get; set; }
    }

    public class OpenWalletInternalRequest
    {
        public string userUid { get; set; }
    }

    public class Account
    {
        public string bank { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string senderaccountnumber { get; set; }
        public string sendername { get; set; }
    }

    public class Order
    {
        public string amount { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
    }

    public class Transaction
    {
        public string reference { get; set; }
        public string sessionId { get; set; }
    }

    public class NinePSBWebhook
    {
        public string code { get; set; }
        public string message { get; set; }
        public string sourceaccount { get; set; }
        public string amount { get; set; }
        public string merchant { get; set; }
        public string sourcebank { get; set; }
        public string sendername { get; set; }
        public string nipsessionid { get; set; }
        public string accountnumber { get; set; }
        public string narration { get; set; }
        public string transactionref { get; set; }
        public string orderref { get; set; }
    }

    public class NinePSBWebhookResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public string transactionref { get; set; }
    }
}