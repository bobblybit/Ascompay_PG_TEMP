using Ascom_Pay_Middleware.Filters;
using AscomPayPG.Data;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Models.WAAS;
using AscomPayPG.Services.Interface;
using Common.Extensions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AscomPayPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOfflineSettlementService _offlineSettlemen;

        public TransactionController(ITransactionService transactionService, AppDbContext context,
                                     IHttpContextAccessor httpContextAccessor,
                                    IOfflineSettlementService offlineSettlemen)
        {
            _transactionService = transactionService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _offlineSettlemen = offlineSettlemen;
        }
        /// <summary>
        ///   This endpoint gets all the accounts attached to a user
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
        [HttpPost("webreceiver-gtb")]
        public async Task<IActionResult> WebHookValidation([FromBody] VirtualAccount_VM model, [FromHeader(Name = "x-squad-signature")] string x_squad_signature)
        {
            var response = await _transactionService.WebhookReceiver(model, x_squad_signature);
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
        ///   This endpoint gets all the accounts attached to a user
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
        [HttpPost("vasreceiver")]
        public async Task<IActionResult> VasWebHookValidation([FromBody] NinePSBWebhook model)
        {
            var response = await _transactionService.WebhookReceiver9PSB(model);
            if (response.success == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        ///   This endpoint gets all the accounts attached to a user
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
        [HttpPost("webreceiver")]
        public async Task<IActionResult> WebHookValidation([FromBody] NinePSBWebhook model)
        {
            var response = await _transactionService.WebhookReceiver9PSB(model);
            if (response.success == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        ///   This endpoint gets all the banks available on the system
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
        [HttpGet("getbanks")]
        public async Task<IActionResult> BankList()
        {
            var response = await _transactionService.Banks();
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
        ///   This endpoint verifies account number
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
        [HttpPost("accountlookup")]
        [ServiceFilter(typeof(UserSessionValidator))]
        public async Task<IActionResult> AccountLookup([FromBody] accountLookupRequest model)
        {
            string userUId = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<string>("UserUid");

            var response = await _transactionService.AccountLookup(model, userUId);
            if (response.IsSuccessful == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }


        [HttpPost("offline-settlement/{userId}")]
        public async Task<IActionResult> OfflineSettlement([FromBody] OfflinePaymentSettlementRequestDTO mdoel, [FromRoute] string userId)
           => Ok(await _offlineSettlemen.ProcessOfflineSettlement(mdoel, userId));

        /* /// <summary>
         ///   This endpoint transfers fund from user virtaul account to selected destination account
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
         [HttpPost("transferfund")]
         public async Task<IActionResult> FundTransfer([FromBody] FundTransfer model, [FromHeader] string userUid)
         {
             var response = await _transactionService.TransferFund(model, userUid);
             if (response.IsSuccessful == true)
             {
                 return Ok(response);
             }
             else
             {
                 return BadRequest(response);
             }
         }*/

    }
}
