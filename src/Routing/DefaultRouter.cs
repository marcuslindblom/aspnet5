using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Routing;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.WebEncoders;

namespace src.Routing
{
    public class DefaultRouter : IRouter
    {
        private readonly IRouter _next;
        private readonly IRouteResolver _routeResolver;
        private readonly IVirtualPathResolver _virtualPathResolver;
        private readonly RequestCulture _defaultRequestCulture;
        private RouteOptions _options;

        public const string ControllerKey = "controller";
        public const string ActionKey = "action";
        public const string DefaultAction = "Index";
        public const string CurrentNodeKey = "currentNode";
        public const string CultureKey = "culture";
        public const string CurrentPageKey = "currentPage";

        public DefaultRouter(IRouter next, IRouteResolver routeResolver, IVirtualPathResolver virtualPathResolver, RequestCulture defaultRequestCulture)
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

            if (defaultRequestCulture == null)
            {
                throw new ArgumentNullException(nameof(defaultRequestCulture));
            }

            _next = next;
            _routeResolver = routeResolver;
            _virtualPathResolver = virtualPathResolver;
            _defaultRequestCulture = defaultRequestCulture;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            EnsureOptions(context.Context);

            var requestCulture = DetectRequestCulture(context.Context);
            var path = _virtualPathResolver.Resolve(context, _defaultRequestCulture, requestCulture);
            if (!path.HasValue)
            {
                // We just want to act as a pass-through for link generation
                return _next.GetVirtualPath(context);
            }
          
            var virtualPathData = new VirtualPathData(_next, path);

            context.IsBound = true;

            return NormalizeVirtualPath(virtualPathData);
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

                context.HttpContext .Items[CurrentNodeKey] = result.TrieNode;
            }

            await _next.RouteAsync(context);
        }

        private RequestCulture DetectRequestCulture(HttpContext context)
        {
            var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
            return requestCultureFeature.RequestCulture;
        }

        private VirtualPathData NormalizeVirtualPath(VirtualPathData pathData)
        {
            if (pathData == null)
            {
                return pathData;
            }

            var url = pathData.VirtualPath;

            if (!string.IsNullOrEmpty(url) && (_options.LowercaseUrls || _options.AppendTrailingSlash))
            {
                var indexOfSeparator = url.Value.IndexOfAny(new char[] { '?', '#' });
                var urlWithoutQueryString = url;
                var queryString = string.Empty;

                if (indexOfSeparator != -1)
                {
                    urlWithoutQueryString = url.Value.Substring(0, indexOfSeparator);
                    queryString = url.Value.Substring(indexOfSeparator);
                }

                if (_options.LowercaseUrls)
                {
                    urlWithoutQueryString = urlWithoutQueryString.Value.ToLowerInvariant();
                }

                if (_options.AppendTrailingSlash && !urlWithoutQueryString.Value.EndsWith("/"))
                {
                    urlWithoutQueryString += "/";
                }

                // queryString will contain the delimiter ? or # as the first character, so it's safe to append.
                url = urlWithoutQueryString + queryString;

                return new VirtualPathData(pathData.Router, url, pathData.DataTokens);
            }

            return pathData;
        }

        private void EnsureOptions(HttpContext context)
        {
            if (_options == null)
            {
                _options = context.RequestServices.GetRequiredService<IOptions<RouteOptions>>().Value;
            }
        }
    }
}