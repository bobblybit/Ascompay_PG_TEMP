using AscomPayPG.Data;
using AscomPayPG.Models;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Gateways;
using AscomPayPG.Services.Interface;

namespace AscomPayPG.Services.Implementation
{
    public class WalletService : IWalletService
    {
        private IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IClientRequestRepository<ClientRequest> _clientRequestRepo;
    
        WAAS waas;

        public WalletService(IConfiguration configuration,
                            IClientRequestRepository<ClientRequest> clientRequestRepo,
                            AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _clientRequestRepo = clientRequestRepo;
            waas = new WAAS(_configuration,context, _clientRequestRepo);
        }
        public async Task<PlainResponse> AccessToken()
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.GetAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> OpenWallet(string userUid)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.OpenWallet(userUid);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletEnquiry(WalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletEnquiry(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletStatus(WalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletStatus(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> GetWallet(WalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.GetWallet(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletTransactions(WalletTransactionRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletTransactions(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> ChangeWalletStatus(ChangeWalletStatusRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.ChangeWalletStatus(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletRequery(WalletRequeryRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletRequery(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletDebit(DebitWalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletDebit(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
        public async Task<PlainResponse> WalletCredit(CreditWalletRequest model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.WalletCredit(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }//-------> from contrller 1
        public async Task<PlainResponse> TransferOtherBank(OtherBankTransferDTO model)
        {
            PlainResponse response = new PlainResponse();
            try
            {
                response = await waas.TransferOtherBank(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                response.IsSuccessful = false;
                response.Message = ex.Message;
                response.ResponseCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
    }
}

