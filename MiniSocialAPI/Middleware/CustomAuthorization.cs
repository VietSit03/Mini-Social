using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MiniSocialAPI.MiniSocialAPI.Middleware
{
    public class CustomAuthorization : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _requiredClaims;
        private readonly string[] _requiredRoles;

        public CustomAuthorization(string[] requiredClaims = null, string[] requiredRoles = null)
        {
            _requiredClaims = requiredClaims;
            _requiredRoles = requiredRoles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }
    }
}

