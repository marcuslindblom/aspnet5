using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Mvc;
using src.Routing.Trie;

namespace src.Components
{
    [ViewComponent(Name = "Menu")]
    public class MenuComponent: ViewComponent
    {
        private readonly IRouteResolverTrie _routeResolverTrie;
        private readonly IHttpContextAccessor _httpAccessor;

        public MenuComponent(IRouteResolverTrie routeResolverTrie, IHttpContextAccessor httpAccessor)
        {
            _routeResolverTrie = routeResolverTrie;
            _httpAccessor = httpAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var requestCultureFeature = _httpAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
            var trie = await _routeResolverTrie.LoadTrieAsync(requestCultureFeature.RequestCulture);

            return View(trie.ChildrenOf("/", true));
        }
    }
}
