using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Raven.Client;

namespace src.Routing.Trie
{
    public class RouteResolverTrie : IRouteResolverTrie
    {
        private readonly IDocumentStore _documentStore;
        private readonly IHttpContextAccessor _accessor;

        public RouteResolverTrie(IDocumentStore documentStore, IHttpContextAccessor accessor)
        {
            _documentStore = documentStore;
            _accessor = accessor;
        }

        public async Task<Trie> LoadTrieAsync()
        {
            var requestCultureFeature = _accessor.HttpContext.Features.Get<IRequestCultureFeature>();
            using (var session = _documentStore.OpenAsyncSession())
            {
                using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(60)))
                {
                    var site = await session.LoadAsync<Site>("sites/" + requestCultureFeature.RequestCulture.Culture.TwoLetterISOLanguageName);
                    return site.Trie;
                }
            }
        }
    }
}