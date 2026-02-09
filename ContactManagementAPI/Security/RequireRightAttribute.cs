using System.Threading.Tasks;
using ContactManagementAPI.Models;
using ContactManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManagementAPI.Security
{
    public class RequireRightAttribute : TypeFilterAttribute
    {
        public RequireRightAttribute(string rightKey) : base(typeof(RequireRightFilter))
        {
            Arguments = new object[] { rightKey };
        }
    }

    public class RequireRightFilter : IAsyncActionFilter
    {
        private readonly string _rightKey;
        private readonly UserContextService _userContext;
        private readonly AuthorizationService _authorizationService;

        public RequireRightFilter(string rightKey, UserContextService userContext, AuthorizationService authorizationService)
        {
            _rightKey = rightKey;
            _userContext = userContext;
            _authorizationService = authorizationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_userContext.UserId.HasValue)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
                return;
            }

            var hasRight = _authorizationService.HasRight(_userContext.UserId.Value, _rightKey);
            if (!hasRight)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            await next();
        }
    }
}
