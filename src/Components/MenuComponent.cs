using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using src.Routing.Trie;

namespace src.Components
{
    [ViewComponent(Name = "Menu")]
    public class MenuComponent: ViewComponent
    {
        private readonly IRouteResolverTrie _routeResolverTrie;

        public MenuComponent(IRouteResolverTrie routeResolverTrie)
        {
            _routeResolverTrie = routeResolverTrie;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var trie = await _routeResolverTrie.LoadTrieAsync();

            return View(trie.ChildrenOf("/", true));
        }
    }
}
