using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using src.Routing;

namespace src.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class PublishedFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(AuthorizationContext context)
        {
            object value;
            if (context.HttpContext.Items.TryGetValue(DefaultRouter.CurrentNodeKey, out value))
            {
                var accessor = context.HttpContext.RequestServices.GetService<IBricsContextAccessor>();
                var currentPage = accessor.CurrentPage;

                if (!currentPage.PublishedDate.HasValue)
                {
                    context.Result = new HttpNotFoundResult();
                }

                if (currentPage.PublishedDate != null && currentPage.PublishedDate.Value > DateTime.Now)
                {
                    context.Result = new HttpNotFoundResult();
                }
            }
        }
    }
}
