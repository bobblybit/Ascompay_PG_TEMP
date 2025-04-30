using AscomPayPG.Data;
using AscomPayPG.Filters;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Filters;
using AscomPayPG.Services.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AscomPayPG.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly AppDbContext _context;
        private readonly IAccountService _accountService;

        public WalletController(IWalletService walletService, AppDbContext context, ITransactionService transactionService, IAccountService accountService)
        {
            _walletService = walletService;
            _context = context;
            _accountService = accountService;
        }
        /// <summary>
        ///   This endpoint creates access token for authentication of other waas endpoints
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        
        /// <summary>
        ///   This endpoint creates 9PSB wallet
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("open-wallet")]
        public async Task<IActionResult> OpenWallet([FromBody] OpenWalletInternalRequest model)
        {
            var response = await _walletService.OpenWallet(model.userUid);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        ///   This endpoint creates upgrades accounts wallet
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("upgrade-tier-3-multi-part")]
        public async Task<IActionResult> WalletUpgradeTier3multipart([FromBody] WalletUpgradeTier3Request model)
        {
            var response = await _walletService.WalletUpgradeTier3multipart(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }


        /// <summary>
        ///   This endpoint gets wallet details using walletNo
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("wallet-enquiry")]
        [ServiceFilter(typeof(UserSessionValidator))]
        public async Task<IActionResult> WalletEnquiry([FromBody] WalletRequest model)
        {
            var response = await _walletService.WalletEnquiry(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        ///   This endpoint gets wallet status using walletNo
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("wallet-status")]
        public async Task<IActionResult> WalletStatus([FromBody] WalletRequest model)
        {
            var response = await _walletService.WalletStatus(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        /// <summary>
        ///   This endpoint gets wallet details using walletNo
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("get-wallet")]
        public async Task<IActionResult> GetWallet([FromBody] WalletRequest model)
        {
            var response = await _walletService.GetWallet(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        /// <summary>
        ///   This endpoint changes wallet status using walletNo
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("change-wallet-status")]
        public async Task<IActionResult> ChangeWalletStatus([FromBody] ChangeWalletStatusRequest model)
        {
            var response = await _walletService.ChangeWalletStatus(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        /// <summary>
        ///   This endpoint changes wallet status using walletNo
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("wallet-transactions")]
        public async Task<IActionResult> WalletTransactions([FromBody] WalletTransactionRequest model)
        {
            var response = await _walletService.WalletTransactions(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
       
        /// <summary>
        ///   This endpoint requery's wallet transactions
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("wallet-requery")]
        public async Task<IActionResult> WalletRequery([FromBody] WalletRequeryRequest model)
        {
            var response = await _walletService.WalletRequery(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        ///   This endpoint debit wallet transactions
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("wallet-debit")]
        public async Task<IActionResult> WalletDebit([FromBody] DebitWalletRequest model)
        {
            var response = await _walletService.WalletDebit(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        ///   This endpoint credit's wallet transactions
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("wallet-credit")]
        public async Task<IActionResult> WalletCredit([FromBody] CreditWalletRequest model)
        {
            var response = await _walletService.WalletCredit(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        ///   This endpoint initaite transfer to other banks using 9psb
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("transfer-other-bank-9psb")]
        [ServiceFilter(typeof(UserSessionValidator))]
        [ServiceFilter(typeof(ExternalTransactionValidator))]
        public async Task<IActionResult> TransferOtherBanks([FromBody] OtherBankTransferDTO model)
        {
            var response = await _walletService.TransferOtherBank(model);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }


        /// <summary>
        ///   update account upgrade grade
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Bad Request</response>
        /// <response code="415">method not allowed, This response is returned when this endpoint is called with HHTP Method other than a Get()</response>
        /// <response code="500">Internal server error</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("upgrade-request-update")]
        public async Task<IActionResult> WalletUpgradeTier3multipart([FromBody] UpdateUpgradeRequestDTO model)
        {
            var response = await _accountService.UpdateTierAccountUpgradeStatus(model.RequestId, model.NewStatus, model.RejectionComment);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}