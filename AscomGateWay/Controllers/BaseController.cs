//using AscomPayPG.Models.Shared;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace AscomPayPG.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BaseController : ControllerBase
//    {
//        internal string GetLoginUserId() => HttpContext.Session.GetString("userUid");
//        public IActionResult BuildHttpResponse<T>(ApiBaseResponse<T> requestReponse)
//        {
//            return requestReponse.ResponseCode switch
//            {
//                StatusCodes.Status200OK => Ok(requestReponse),
//                StatusCodes.Status201Created => Created("", requestReponse),
//                StatusCodes.Status404NotFound => NotFound(requestReponse),
//                StatusCodes.Status401Unauthorized => Unauthorized(requestReponse),
//                StatusCodes.Status400BadRequest => BadRequest(requestReponse),
//                StatusCodes.Status413PayloadTooLarge => BadRequest(requestReponse),
//                StatusCodes.Status415UnsupportedMediaType => BadRequest(requestReponse),
//                StatusCodes.Status204NoContent => NoContent(),
//            };
//        }
//    }
//}
