using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using src.Routing;

namespace src.Mvc
{
    public class AuthorizeFilterAttribute : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!HasAllowAnonymous(context))
            {
                object value;
                if (context.HttpContext.Items.TryGetValue(DefaultRouter.CurrentNodeKey, out value))
                {

                    var accessor = context.HttpContext.RequestServices.GetService<IBricsContextAccessor>();
                    var currentPage = accessor.CurrentPage;

                    if (currentPage?.Acl == null || currentPage.Acl == AccessControl.Anonymous)
                    {
                        return;
                    }

                    if (currentPage.Acl == AccessControl.Authenticated && !context.HttpContext.User.Identity.IsAuthenticated)
                    {
                        context.Result = new NotFoundResult();
                    }

                    if (currentPage.Acl == AccessControl.Administrators && !context.HttpContext.User.IsInRole(Enum.GetName(typeof(AccessControl), currentPage.Acl)))
                    {
                        context.Result = new NotFoundResult();
                    }
                }
            }
        }

        public bool HasAllowAnonymous(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            return context.Filters.Any(item => item is IAllowAnonymousFilter);
        }
    }

    public enum AccessControl {
        Anonymous = 0,
        Authenticated = 1,
        Administrators = 2
    }
}
