using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Raven.Client;
using Raven.Client.Documents;
using src.Localization;
using src.Routing.Trie;

namespace src.Components
{
    [ViewComponent(Name = "Menu")]
    public class MenuComponent: ViewComponent
    {
        private readonly IRouteResolverTrie _routeResolverTrie;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IDocumentStore _documentStore;

        public MenuComponent(IRouteResolverTrie routeResolverTrie, IHttpContextAccessor httpAccessor, IDocumentStore documentStore)
        {
            _routeResolverTrie = routeResolverTrie;
            _httpAccessor = httpAccessor;
            _documentStore = documentStore;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var requestCultureFeature = _httpAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
            var trie = await _routeResolverTrie.LoadTrieAsync(requestCultureFeature.RequestCulture);

            using (var session = _documentStore.OpenAsyncSession())
            {
                var ids = trie.ChildrenOf("/", true).Select(x => x.Value.PageId);
                //var query = await session.LoadAsync<Page>(ids);
                var pages = await session.LocalizeFor(requestCultureFeature.RequestCulture.Culture).LoadAsync(ids);
                //var pages = query.Select(x => x.Value).ToList();
                return View(pages);
            }

            //return View(trie.ChildrenOf("/", true));
        }
    }
}
