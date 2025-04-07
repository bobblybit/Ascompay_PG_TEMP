using AscomPayPG.Services;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ascom_Pay_Middleware.Filters
{
    public class UserSessionValidator : IAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHelperService _helperService;

        public UserSessionValidator(IHttpContextAccessor httpContextAccessor, IHelperService helperService)
        {
            _httpContextAccessor = httpContextAccessor;
            _helperService = helperService;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var authHeader = request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Look up the user based on the token
            var user = _helperService.GetUserBySessionAsync(token).GetAwaiter().GetResult(); 

            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var session = context.HttpContext.Session;
            session.SetObjectAsJson("Email", user.Email);
            session.SetObjectAsJson("UserUid", user.UserUid);
        }
    }
}
