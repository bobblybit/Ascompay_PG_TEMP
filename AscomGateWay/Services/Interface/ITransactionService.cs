using AscomPayPG.Models.DTO;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;

namespace AscomPayPG.Services.Interface
{
    public interface ITransactionService
    {
      //  Task<TransferResponseDTO> TransferFundFromAccountOrWalletToAccount(TransferRequestDTO requestModel);
        Task<PlainResponse> TransferFundInternal(TransferRequestDTO requestModel);
        public Task<PlainResponse> WebhookReceiver(VirtualAccount_VM payload, string x_squad_signature);
        public Task<PlainResponse> WebhookReceiver9PSB(NinePSBWebhook payload);
        public Task<PlainResponse> Banks();
        public Task<AccountLookUpResponse> AccountLookup(accountLookupRequest accountLookupRequest, string userUid);
        public Task<PlainResponse> TransferFund(FundTransfer model, string userUid);
        Task<decimal> UpdateSourceAccountBalance(Models.DTO.Account account, decimal amount, bool isFromAccount = false);
    }
}
