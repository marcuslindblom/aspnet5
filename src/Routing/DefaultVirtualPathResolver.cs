using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Routing;
using src.Routing.Trie;
using Microsoft.AspNet.Http.Features;
using src.Mvc;

namespace src.Routing
{
	public interface IVirtualPathResolver
	{
        PathString Resolve(VirtualPathContext virtualPathContext);
	}
	
	public class DefaultVirtualPathResolver : IVirtualPathResolver
	{
        private IRouteResolverTrie _routeResolverTrie;
        private IControllerMapper _controllerMapper;
	    private readonly IHttpContextAccessor _httpAccessor;

	    public DefaultVirtualPathResolver(IRouteResolverTrie routeResolverTrie, IControllerMapper controllerMapper, IHttpContextAccessor httpAccessor)
        {
            _routeResolverTrie = routeResolverTrie;
            _controllerMapper = controllerMapper;
	        _httpAccessor = httpAccessor;
        }

	    public PathString Resolve(VirtualPathContext virtualPathContext)
	    {
	        object culture;
	        if (!virtualPathContext.Values.TryGetValue(DefaultRouter.CultureKey, out culture))
	        {
	            var requestCultureFeature = _httpAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
	            culture = requestCultureFeature.RequestCulture;
	        }

            // Handle when the page is added as a route parameter
            object value;
	        if (virtualPathContext.Values.TryGetValue("page", out value))
	        {
	            var page = value as Page;
	            if (page == null) return null;
	        
                var trie = _routeResolverTrie.LoadTrieAsync();
                trie.Wait();

	            var node = trie.Result.FirstOrDefault(n => n.Value.PageId == page.Id);

                return new PathString(node.Key);
            }

	        return null;
	    }
	}
}