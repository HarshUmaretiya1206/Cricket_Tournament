using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CricHeroesClone.Attributes
{
    public class AuthorizeRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeRoleAttribute(string role) : base(typeof(AuthorizeRoleFilter))
        {
            Arguments = new object[] { role };
        }
    }

    public class AuthorizeRoleFilter : IAuthorizationFilter
    {
        private readonly string _role;

        public AuthorizeRoleFilter(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role) || role != _role)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
