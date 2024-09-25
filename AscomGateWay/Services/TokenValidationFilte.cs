using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AscomPayPG.Services
{
    public class ValidateCustomTokenAttribute : TypeFilterAttribute
    {
        public ValidateCustomTokenAttribute() : base(typeof(ValidateCustomTokenFilter))
        {
        }
    }

    public class ValidateCustomTokenFilter : IActionFilter
    {
        private readonly IHelperService _ihelper;
        private IHttpContextAccessor _httpContextAccessor;

        public ValidateCustomTokenFilter(IHelperService ihelper, IHttpContextAccessor httpContextAccessor)
        {
            _ihelper = ihelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing on action executed
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            // Get the token from the request headers or query parameters
            string token = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string searchString = "Bearer ";

            string modifiedString = token.Replace(searchString, "");

            var res = await _ihelper.SessionValidation(modifiedString);

            if (res.isOk == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            else
            {
                SetToken(modifiedString);
            }
        }


        public void SetToken(string token)
        {
            _httpContextAccessor.HttpContext.Session.SetString("token", token);
            /*if (_httpContextAccessor?.HttpContext?.Session?.Keys.FirstOrDefault() == null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("token", token);
            } */           
        }

    }
}