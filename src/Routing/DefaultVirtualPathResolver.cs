using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Routing;
using src.Routing.Trie;
using src.Mvc;

namespace src.Routing
{
	public interface IVirtualPathResolver
	{
        PathString Resolve(VirtualPathContext virtualPathContext, RequestCulture defaultRequestCulture, RequestCulture requestCulture);
	}
	
	public class DefaultVirtualPathResolver : IVirtualPathResolver
	{
        private IRouteResolverTrie _routeResolverTrie;
        private IControllerMapper _controllerMapper;

        public DefaultVirtualPathResolver(IRouteResolverTrie routeResolverTrie, IControllerMapper controllerMapper)
        {
            _routeResolverTrie = routeResolverTrie;
            _controllerMapper = controllerMapper;
        }

	    public PathString Resolve(VirtualPathContext virtualPathContext, RequestCulture defaultRequestCulture, RequestCulture requestCulture)
	    {
            // Handle when the page is added as a route parameter
            object value;
	        if (virtualPathContext.Values.TryGetValue("page", out value))
	        {
	            var page = value as Page;
	            if (page == null) return null;

	            object culture;
	            if (virtualPathContext.Values.TryGetValue("culture", out culture))
	            {
	                string cultureValue = culture as string;
	                if (cultureValue != null)
	                {
                        requestCulture = new RequestCulture(cultureValue);
                    }	                
	            }
	        
                var trie = _routeResolverTrie.LoadTrieAsync(requestCulture);
                trie.Wait();

	            var node = trie.Result.FirstOrDefault(n => n.Value.PageId == page.Id);

	            PathString path = new PathString();
	            if (defaultRequestCulture.Culture.LCID != requestCulture.Culture.LCID)
	            {
	                path = path.Add("/" + requestCulture.Culture.TwoLetterISOLanguageName);
	            }

	            return path.Add(node.Key);
	        }

            if (virtualPathContext.Values.TryGetValue("id", out value))
            {
                var id = value as string;
                if (id == null) return null;

                object culture;
                if (virtualPathContext.Values.TryGetValue("culture", out culture))
                {
                    string cultureValue = culture as string;
                    if (cultureValue != null)
                    {
                        requestCulture = new RequestCulture(cultureValue);
                    }
                }

                var trie = _routeResolverTrie.LoadTrieAsync(requestCulture);
                trie.Wait();

                var node = trie.Result.FirstOrDefault(n => n.Value.PageId == id);

                PathString path = new PathString();
                if (defaultRequestCulture.Culture.LCID != requestCulture.Culture.LCID)
                {
                    path = path.Add("/" + requestCulture.Culture.TwoLetterISOLanguageName);
                }

                return path.Add(node.Key);
            }

            return null;
	    }
    }
}