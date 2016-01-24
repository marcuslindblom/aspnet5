using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using src.Routing;

namespace src.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(AuthorizationContext context)
        {
            if(!HasAllowAnonymous(context)) {
                object value;
                if(context.HttpContext.Items.TryGetValue(DefaultRouter.CurrentNodeKey, out value))
                {

                    var accessor = context.HttpContext.RequestServices.GetService<IBricsContextAccessor>();
                    var currentPage = accessor.CurrentPage;

                    if (currentPage?.Acl == null || currentPage.Acl == AccessControl.Anonymous)
                    {
                        return;
                    }

                    if (currentPage.Acl == AccessControl.Authenticated && !context.HttpContext.User.Identity.IsAuthenticated)
                    {
                        context.Result = new HttpNotFoundResult();
                    }

                    if (currentPage.Acl == AccessControl.Administrators && !context.HttpContext.User.IsInRole(Enum.GetName(typeof(AccessControl), currentPage.Acl)))
                    {
                        context.Result = new HttpNotFoundResult();
                    }
                }
            }
        }
    }

    public enum AccessControl {
        Anonymous = 0,
        Authenticated = 1,
        Administrators = 2
    }
}
