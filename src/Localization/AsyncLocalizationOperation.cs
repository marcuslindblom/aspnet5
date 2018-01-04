using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using src.Routing.Trie;

namespace src.Localization
{
    public class AsyncLocalizationOperation : IAsyncLocalizationOperation
    {
        private readonly IAsyncDocumentSession _session;
        private readonly RequestCulture _requestCulture;

        public string Key { get; set; }

        public object Entity { get; set; }

        public Page Page { get; set; }

        public AsyncLocalizationOperation(IAsyncDocumentSession session, RequestCulture requestCulture)
        {
            _session = session;
            _requestCulture = requestCulture;
        }

        public Task<Page> LoadAsync(string id, CancellationToken token = default(CancellationToken))
        {            
            var postfix = $"/{_requestCulture.Culture.TwoLetterISOLanguageName}";
            var query = from page in _session.Query<Page>()
                        where page.Id == id
                        let localizedDocument = RavenQuery.Load<Page>(page.Id + postfix)
                        select new Page
                        {
                            Id = page.Id,
                            Name = localizedDocument.Name,
                            PublishedDate = page.PublishedDate,
                            Acl = page.Acl,
                            Metadata = localizedDocument.Metadata
                        };

            return query.FirstOrDefaultAsync();
        }

        public Task<List<Page>> LoadAsync(IEnumerable<string> ids, CancellationToken token = default(CancellationToken))
        {
            var postfix = $"/{_requestCulture.Culture.TwoLetterISOLanguageName}";
            var query = from page in _session.Query<Page>()
                        where page.Id.In(ids)
                        let localizedDocument = RavenQuery.Load<Page>(page.Id + postfix)
                        select new Page
                        {
                            Id = page.Id,
                            Name = localizedDocument.Name,
                            PublishedDate = page.PublishedDate,
                            Acl = page.Acl,
                            Metadata = localizedDocument.Metadata
                        };

            return query.ToListAsync();
        }

        public Task StoreAsync(Page localizedPage, CancellationToken token = default(CancellationToken))
        {
            if (localizedPage == null) return null;

            var page = Page ?? new Page();

            //page.Acl = localizedPage.Acl;

            localizedPage.Acl = null;

            _session.StoreAsync(page, token).Wait(token);

            Task<Site> siteTask = _session.LoadAsync<Site>("sites/" + _requestCulture.Culture.TwoLetterISOLanguageName, token);

            return Task.WhenAll(siteTask).ContinueWith(task =>
            {
                if (!string.IsNullOrEmpty(Key))
                {
                    var trie = siteTask.Result.Trie;

                    // Update an existing item
                    if (trie.Any(x => x.Value.PageId == page.Id))
                    {
                        var nodeToRemove = trie.SingleOrDefault(x => x.Value.PageId == page.Id);
                        trie.Remove(nodeToRemove.Key);
                        trie.Add(Key, new TrieNode(page.Id, nodeToRemove.Value.ControllerName));
                    }

                    // Don't add the same key
                    else if (!trie.ContainsKey(Key))
                    {
                        trie.Add(Key, new TrieNode(page.Id, Entity.GetType().Name));
                    }
                }

                _session.StoreAsync(localizedPage, string.Join("/", page.Id, _requestCulture.Culture.TwoLetterISOLanguageName), token);

                _session.StoreAsync(Entity, string.Join("/", page.Id, _requestCulture.Culture.TwoLetterISOLanguageName, "content"), token);

            }, token);
        }
    }
}