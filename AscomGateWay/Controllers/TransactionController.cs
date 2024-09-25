using AscomPayPG.Data;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.DTOs;
using AscomPayPG.Models.GTPay;
using AscomPayPG.Services.Interface;
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

        public TransactionController(ITransactionService transactionService, AppDbContext context)
        {
            _transactionService = transactionService;
            _context = context;
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
        public async Task<IActionResult> AccountLookup([FromBody] accountLookupRequest model, [FromHeader] string userUid)
        {
            var response = await _transactionService.AccountLookup(model, userUid);
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
        }

    }
}
