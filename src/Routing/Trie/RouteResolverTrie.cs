using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Raven.Client.Documents;

namespace src.Routing.Trie
{
    public class RouteResolverTrie : IRouteResolverTrie
    {
        private readonly IDocumentStore _documentStore;

        public RouteResolverTrie(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<Trie> LoadTrieAsync(RequestCulture requestCulture)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(60)))
                {
                    var site = await session.LoadAsync<Site>("sites/" + requestCulture.Culture.TwoLetterISOLanguageName);
                    return site.Trie;
                }
            }
        }
    }
}