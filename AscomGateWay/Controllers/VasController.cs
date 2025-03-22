using AscomPayPG.Models.VasModels;
using AscomPayPG.Services.Filters;
using AscomPayPG.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AscomPayPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VasController : ControllerBase
    {
        private readonly IVasService _vasService;
        public VasController(IVasService vasService)
        {
            _vasService = vasService;
        }


        #region TOP-UP

        /// <summary>
        ///   This endpoint gets a phone number network provider
        /// </summary>
        /// <param name="phoneNumber"></param>
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
        [HttpGet("phone-network")]
        public async Task<IActionResult> GetPhoneNumberNetwork([FromQuery] string phoneNumber)
        {
            var response = await _vasService.GetPhoneNumberNetwork(phoneNumber);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        /// <summary>
        ///   This endpoint gets all the data plans assocaited with a network
        /// </summary>
        /// <param name="phoneNumber"></param>
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
        [HttpGet("data-plans")]
        public async Task<IActionResult> PurchaseData([FromQuery] string phoneNumber)
        {
            var response = await _vasService.GetDataPlans(phoneNumber);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        /// <summary>
        ///   This endpoint get transaction status
        /// </summary>
        /// <param name="transactionReference"></param>
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
        [HttpGet("transaction-status")]
        public async Task<IActionResult> GetTopUpStatus([FromQuery] string transactionReference)
        {
            var response = await _vasService.GetTopUpStatus(transactionReference);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        /// <summary>
        ///   This endpoint gets all the purchase airtime
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
        [HttpPost("airtime-top-up")]
        [ServiceFilter(typeof(VasTransactionValidator))]
        public async Task<IActionResult> PurchaseAirtime([FromBody] AirTimeTopUpRequest model, [FromQuery]string userId)
        {
            var response = await _vasService.PurchaseAirtime(model, userId);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        /// <summary>
        ///   This endpoint gets all the purchase data
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
        [HttpPost("data-top-up")]
        [ServiceFilter(typeof(VasTransactionValidator))]
        public async Task<IActionResult> PurchaseData([FromBody] DataTopUpRequest model, [FromQuery] string userId)
        {
            var response = await _vasService.PurchaseData(model, userId);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }
        #endregion

        /// <summary>
        ///  This endpoint provides a list of bills payment categories available to client
        /// </summary>
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
        [HttpGet("get-category")]
        public async Task<IActionResult> GetCategory()
        {
            var response = await _vasService.GetCategory();
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        /// <summary>
        ///  This endpoint returns the status of a billspayment transaction. The transaction reference used to initiate the bills payment transaction is passed as a request parameter to the URL as shown below
        /// </summary>
        ///  <param name="transactionReference"></param>
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
        [HttpGet("get-category/{transactionReference}")]
        public async Task<IActionResult> GetBillerPaymentStatus([FromRoute] string transactionReference)
        {
            var response = await _vasService.GetBillerPaymentStatus(transactionReference);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        /// <summary>
        ///  This endpoint provides a list of billers under a category
        /// </summary>
        ///  <param name="categoryId"></param>
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
        [HttpGet("get-category-biller/{categoryId}")]
        public async Task<IActionResult> GetCategoryBiller([FromRoute] string categoryId)
        {
            var response = await _vasService.GetCategoryBiller(categoryId);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        /// <summary>
        ///  this endpoint returns an outline of input parameters required to process payment for a biller.
        /// </summary>
        ///  <param name="billerId"></param>
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
        [HttpGet("biller-input-field/{billerId}")]
        public async Task<IActionResult> GetBillerInputFields([FromRoute] string billerId)
        {
            var response = await _vasService.GetBillerInputFields(billerId);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        /// <summary>
        ///  This endpoint validates the user input or selection from GET BILLER INPUT FIELDS. It returns basic info about a customer based on the input fields.
        /// </summary>
        /// <param name="requestModel"></param>
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
        [HttpPost("biller-input-validation")]
        public async Task<IActionResult> VaildateBillerInputFields([FromBody] ValidateBillerInputRequest requestModel)
        {
            var response = await _vasService.VaildateBillerInputFields(requestModel);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }

        
        /// <summary>
        ///  This endpoint initiates payment for VAS transactions. Some values from previous endpoints are required to build request for this endpoint.
        /// </summary>
        /// <param name="requestModel"></param>
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
        [HttpPost("biller-payment")]
        [ServiceFilter(typeof(VasTransactionValidator))]
        public async Task<IActionResult> InitBillerPayment([FromBody] InitaiteBillPaymentRequest requestModel)
        {
            var response = await _vasService.InitBillerPayment(requestModel);
            if (response.IsSuccessful == true)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
