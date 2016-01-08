using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client;
using src.Routing.Trie;

namespace src.Models
{
    public class LayoutModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRouteResolverTrie _routeResolverTrie;


        public LayoutModel(IDocumentStore documentStore, IHttpContextAccessor httpContextAccessor, IRouteResolverTrie routeResolverTrie)
        {            
            _httpContextAccessor = httpContextAccessor;
            _routeResolverTrie = routeResolverTrie;
            Initialize(documentStore);
        }

        public async void Initialize(IDocumentStore documentStore)
        {
            var trie = await _routeResolverTrie.LoadTrieAsync(new RequestCulture(LanguageName));
            TrieNode node;
            trie.TryGetNode("/", out node);
            if (node != null)
            {
                Id = node.PageId;
            }
        }

        public string Id { get; set; }

        public string MetaTitle => "The meta title from the page";

        public string MetaDescription => "The meta description from the page";

        public string CanonicalUrl => "https://aspnet5rc.azurewebsites.net/";

        public string LanguageName
        {
            get
            {
                var requestCultureFeature = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
                var requestCulture = requestCultureFeature.RequestCulture;
                return requestCulture.Culture.TwoLetterISOLanguageName;
            }
        }
    }
}
