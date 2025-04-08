using AscomPayPG.Helpers;
using AscomPayPG.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AscomPayPG.Filters
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

            var refreshToken = request.Headers["Token"].FirstOrDefault();

            if (string.IsNullOrEmpty(refreshToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Look up the user based on the token
            var userCurrentSession = _helperService.GetUserCurrentSessionAsync(token, refreshToken).GetAwaiter().GetResult();

            if (userCurrentSession == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (userCurrentSession.TokenExpiry < DateTime.UtcNow)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var session = context.HttpContext.Session;
            session.SetObjectAsJson("Email", userCurrentSession.Email);
            session.SetObjectAsJson("UserUid", userCurrentSession.UserId);
        }
    }
}
