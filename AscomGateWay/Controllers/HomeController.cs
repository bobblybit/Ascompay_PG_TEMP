using System.Diagnostics;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using AscomPayPG.Models.ViewModels;
using AscomPayPG.Services;
using AscomPayPG.Services.Filters;
using AscomPayPG.Services.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AscomPayPG.Controllers
{

    // [Route("Payments")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [Route("[controller]")]
    [ApiController]
    [EnableCors]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITransactionsRepository<Transactions> _TransactionsRepo;
        private readonly IConfiguration _configuration;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly IHelperService _helperService;
        private readonly IRepository<PaymentGateway> _paymentGatewayRepo;
        private readonly ITransactionService _transactionService;
        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            IPaymentProcessor paymentProcessor,
            IHelperService helperService,
            IRepository<PaymentGateway> paymentGatewayRepo,
            ITransactionService transactionService,
            ITransactionsRepository<Transactions> TransactionsRepo)
        {
            _logger = logger;
            _configuration = configuration;
            _paymentProcessor = paymentProcessor;
            _helperService = helperService;
            _paymentGatewayRepo = paymentGatewayRepo;
            _TransactionsRepo = TransactionsRepo;
            _transactionService= transactionService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Notify")]
        public IActionResult Notify()
        {
            return View();
        }

        /* API controller Actions  */
        [HttpGet("PaymentGateways")]
        [ValidateCustomToken]
        public async Task<ActionResult> GetPaymentGateways()
        {
            var url = HttpContext.Request.GetDisplayUrl();
          
            IEnumerable<PaymentGateway> response = new List<PaymentGateway>();

            response = await _paymentGatewayRepo.GetAll();

            return Json(response);
        }

        [HttpGet("TransactionHistories")]
        [ValidateCustomToken]
        public async Task<ActionResult<TransactionsResponse>> GetTransactionsHistories(int page = 1)
        {

            TransactionsResponse response = new TransactionsResponse();

            response = await _TransactionsRepo.GetPaginatedAll(page);

            return response;

        }

        [HttpGet("MyTransactionHistories")]
        [ValidateCustomToken]
        public async Task<ActionResult<TransactionsResponse>> GetMyTransactionsHistories(string uid, int page = 1)
        {

            TransactionsResponse response = new TransactionsResponse();

            response = await _TransactionsRepo.GetPaginatedAll(uid, page);

            return response;

        }

        [HttpGet("PayQuery")]
        [ValidateCustomToken]
        public async Task<ActionResult<PayQueryResponse>> QueryStatus(string reference)
        {
            PayQueryResponse response = new PayQueryResponse();

            response = await _paymentProcessor.QueryStatus(reference);

            return response;

        }
       
        //direct from 3rd party app
        [HttpPost("Pay")]
        [ValidateCustomToken]
        public async Task<IActionResult> Pay([FromBody] PaymentRequest payReq)
        {
            var response = new PayResponse();

            string baseUrl = _configuration["App:BaseUrl"];          
            
            var resp = await _paymentProcessor.Pay(payReq);
            var reference = resp.Data.First().Reference;
            var redirectUrl = $"{baseUrl}/Home/gateway?reference={reference}&status={resp.Status}";

            //var redirectUrl = $"{Request.Scheme}://{Request.Host.Value}/Home/gateway?reference={reference}&status={resp.Status}";

            response.Message = resp.Message;
            response.Status = resp.Status;
            response.RedirectUrl = resp.Status ? redirectUrl : string.Empty;
            //return Json(new { status = resp.Status, message = resp.Message,  data = redirectUrl });
            return Json(response);
        }
       

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Gateway")]
        public async Task<IActionResult> Gateway([FromQuery] string Reference, [FromQuery] bool Status)
        {
            var model = new GatewayViewModel();

            model = await _paymentProcessor.GetGatewayViewData(Reference);
            ViewBag.Message = Status ? string.Empty : "Payment Initialization Failed!";
            ViewBag.Succeeded = Status;
            return View(model);

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PostGateway")]
        public async Task<IActionResult> PostGateway([FromForm] GatewayViewModel form)
        {
            if (form.GatewayId == 0)
            {
                ModelState.AddModelError(string.Empty, "Gateway cannot be empty");
                return View(form);
            }
            //return redirection to confirmation view for the gateway
            var resp = await _paymentProcessor.PayInit(form);
            if (!resp.Status)
            {
                TempData["ErrorMessage"] = resp.Message;
                TempData["CallbackURL"] = resp.Data.First().RequestingClientUrl;
                return RedirectToAction("Error");
            }
            TempData["PayData"] = JsonConvert.SerializeObject(resp.Data.First().Data);


            return RedirectToAction("PayConfirmation", resp.Data.First());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            string clientCallbackUrl = TempData["CallbackURL"]?.ToString()!;
            string message = TempData["ErrorMessage"]?.ToString()!;

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message, ClientCallbackUrl = clientCallbackUrl });
        }

        /* Gateway Confirmation Actions  */
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("PayConfirmation")]
        public IActionResult PayConfirmation([FromQuery] PaymentResponseView model)
        {
            var confirmationPage = $"{model.Gateway}Confirmation";
            model.Data = JsonConvert.DeserializeObject<string>(TempData["PayData"].ToString());
            return View(confirmationPage, model);
        }

        // reference | tranx
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("PayCallBack")]
        public async Task<IActionResult> PayCallBack()
        {
            string reference = Request.QueryString.ToString();
            //Console.WriteLine($"reference: {reference}");
            _logger.LogInformation($"reference: {reference}");

            if (string.IsNullOrEmpty(reference))
            {
                //return Json(new object());//View(nameof(Error)); 
                TempData["ErrorMessage"] = "Reference Code is missing in request query";
                return RedirectToAction(nameof(Error));
            }

            reference = reference.Substring(1).Split("=").Last();

            string? referenceCode = reference;
            PayStatusViewModel model = new PayStatusViewModel();
            model.Reference = reference;
            if (!string.IsNullOrEmpty(referenceCode))
            {
                model = await _paymentProcessor.ProcessCallBack(referenceCode);
            }
            var payStatus = "PayStatus";
            return View(payStatus, model);

        }

        [HttpPost("transfer")]
        [ServiceFilter(typeof(InternalTransactionValidator))]
        public async Task<IActionResult> TransferFund([FromBody] TransferRequestDTO mdoel)
        {
            var reponse = await _transactionService.TransferFundInternal(mdoel);
            return Json(reponse);
        }
    }
}