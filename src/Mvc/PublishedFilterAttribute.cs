using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using src.Routing;

namespace src.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class PublishedFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            object value;
            if (context.HttpContext.Items.TryGetValue(DefaultRouter.CurrentNodeKey, out value))
            {
                var accessor = context.HttpContext.RequestServices.GetService<IBricsContextAccessor>();
                var currentPage = accessor.CurrentPage;

                if (!currentPage.PublishedDate.HasValue)
                {
                    context.Result = new NotFoundResult();
                }

                if (currentPage.PublishedDate != null && currentPage.PublishedDate.Value > DateTime.Now)
                {
                    context.Result = new NotFoundResult();
                }
            }            
        }
    }
}
