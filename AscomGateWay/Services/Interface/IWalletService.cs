using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;

namespace AscomPayPG.Services.Interface
{
    public interface IWalletService
    {
        //public Task<PlainResponse> AccessToken();
        public Task<PlainResponse> OpenWallet(string userUid);
        public Task<PlainResponse> WalletEnquiry(WalletRequest model);
        public Task<PlainResponse> WalletStatus(WalletRequest model);
        public Task<PlainResponse> GetWallet(WalletRequest model);
        public Task<PlainResponse> WalletTransactions(WalletTransactionRequest model);
        public Task<PlainResponse> ChangeWalletStatus(ChangeWalletStatusRequest model);
        public Task<PlainResponse> WalletRequery(WalletRequeryRequest model);
        public Task<PlainResponse> WalletDebit(DebitWalletRequest model);
        public Task<PlainResponse> WalletCredit(CreditWalletRequest model);
        Task<PlainResponse> TransferOtherBank(OtherBankTransferDTO model);
        Task<PlainResponse> WalletUpgradeTier3multipart(WalletUpgradeTier3Request model);
        Task<PlainResponse> WalletUpgradeTier2(WalletUpgradeTier2Request model);


        #region IMPLEMENTATION FOR ASCOMPAY WALLET
        Task<UserWallet> GetIntenalWalletById(string  Id);
        Task<bool> UpdateIntenalWallet(UserWallet walletToUpdate);
        #endregion
    }
}
