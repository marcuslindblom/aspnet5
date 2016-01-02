using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Routing;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;

namespace src.Routing
{
    public class DefaultRouter : IRouter
    {
        private readonly IRouter _next;
        private readonly IRouteResolver _routeResolver;
        private readonly IVirtualPathResolver _virtualPathResolver;

        public const string ControllerKey = "controller";
        public const string ActionKey = "action";
        public const string DefaultAction = "Index";
        public const string CurrentNodeKey = "currentNode";
        public const string CultureKey = "culture";

        public DefaultRouter(IRouter next, IRouteResolver routeResolver, IVirtualPathResolver virtualPathResolver)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (routeResolver == null)
            {
                throw new ArgumentNullException(nameof(routeResolver));
            }

            if (virtualPathResolver == null)
            {
                throw new ArgumentNullException(nameof(virtualPathResolver));
            }

            _next = next;
            _routeResolver = routeResolver;
            _virtualPathResolver = virtualPathResolver;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            var path = _virtualPathResolver.Resolve(context);
            if (!path.HasValue)
            {
                // We just want to act as a pass-through for link generation
                return _next.GetVirtualPath(context);
            }
            var virtualPathData = new VirtualPathData(_next, path);
            context.IsBound = true;
            return virtualPathData;
        }

        public async Task RouteAsync(RouteContext context)
        {
            // Abort and proceed to other routes in the route table if path contains api or ui
            string[] segments = context.HttpContext.Request.Path.Value.Split(new[] { '/' });
            if (segments.Any(segment => segment.Equals("api", StringComparison.OrdinalIgnoreCase) ||
                                        segment.Equals("brics", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            //context.RouteData.Routers.Insert(0,_next);

            var requestCulture = DetectRequestCulture(context.HttpContext);

            IResolveResult result = await _routeResolver.Resolve(context, requestCulture);

            if(result != null) { 
                context.RouteData.Values[ControllerKey] = result.Controller;
                context.RouteData.Values[ActionKey] = result.Action;

                context.HttpContext.Items[CurrentNodeKey] = result.TrieNode;
            }

            await _next.RouteAsync(context);
        }

        private RequestCulture DetectRequestCulture(HttpContext context)
        {
            var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
            return requestCultureFeature.RequestCulture;
        }
    }
}