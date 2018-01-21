using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
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
        private readonly IBricsContextAccessor _bricsContextAccessor;
        private RequestCulture CurrentRequestCulture => _httpAccessor.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture;

        public MenuComponent(IRouteResolverTrie routeResolverTrie, IHttpContextAccessor httpAccessor, IDocumentStore documentStore, IBricsContextAccessor bricsContextAccessor)
        {
            _routeResolverTrie = routeResolverTrie;
            _httpAccessor = httpAccessor;
            _documentStore = documentStore;
            _bricsContextAccessor = bricsContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var trie = await _routeResolverTrie.LoadTrieAsync(CurrentRequestCulture);

            using (var session = _documentStore.OpenAsyncSession())
            {
              var ids = await session.Advanced.GetChildrenOf(_bricsContextAccessor.CurrentPage, CurrentRequestCulture);
              var pages = await session.LocalizeFor(CurrentRequestCulture).LoadAsync(ids);
              return View(pages);

                //var ids = trie.ChildrenOf("/", true).Select(x => x.Value.PageId);
                //var children = await session.Advanced.GetChildrenOf(_bricsContextAccessor.CurrentPage, CurrentRequestCulture);
                //var pages = await session.LocalizeFor(CurrentRequestCulture).LoadAsync(children);
                //var ids = trie.ChildrenOf("/", true).Select(x => x.Value.PageId);
                //var pages = await session.Localize().LoadAsync(ids);
            }
        }
    }

    public static class AsyncAdvancedSessionOperationExtensions {
        public static async Task<IEnumerable<string>> GetChildrenOf(this IAsyncAdvancedSessionOperations asyncAdvancedSessionOperations, Page page, RequestCulture requestCulture) {
            var site = await ((AsyncDocumentSession)asyncAdvancedSessionOperations).LoadAsync<Site>($"sites/{requestCulture.Culture.TwoLetterISOLanguageName}");
            var node = site.Trie.Where(x => x.Value.PageId == page.Id).FirstOrDefault();
            return site.Trie.ChildrenOf(node.Key, true).Select(x => x.Value.PageId);
        }
    }
}
