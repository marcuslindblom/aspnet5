using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using src.Routing.Trie;
using src;

namespace src.Localization
{
    public class AsyncLocalizationOperation : IAsyncLocalizationOperation
    {
        private readonly IAsyncDocumentSession _session;
        private readonly CultureInfo _locale;

        public string Key { get; set; }

        public object Entity { get; set; }

        public Page Page { get; set; }

        public AsyncLocalizationOperation(IAsyncDocumentSession session, CultureInfo locale)
        {
            _session = session;
            _locale = locale;
        }

        public Task<Page> LoadAsync(string id, CancellationToken token = default(CancellationToken))
        {
            var postfix = $"/{_locale.TwoLetterISOLanguageName}";
            var query = from page in _session.Query<Page>()
                        where page.Id == id
                        let localizedDocument = RavenQuery.Load<Page>(page.Id + postfix)
                        select new Page
                        {
                            Id = page.Id,
                            Name = localizedDocument.Name,
                        };

            return query.FirstOrDefaultAsync();

            //return _session.LoadAsync<LocalizationTransformer, T>(id,
                //configuration =>
                //{
                //    configuration.AddTransformerParameter("Locale", _locale.TwoLetterISOLanguageName);
                //}, token);
        }

        public Task<List<Page>> LoadAsync(IEnumerable<string> ids, CancellationToken token = default(CancellationToken))
        {
            var postfix = $"/{_locale.TwoLetterISOLanguageName}";
            var query = from page in _session.Query<Page>()
                        where page.Id.In(ids)
                        let localizedDocument = RavenQuery.Load<Page>(page.Id + postfix)
                        select new Page
                        {
                            Id = page.Id,
                            Name = localizedDocument.Name,
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

            Task<Site> siteTask = _session.LoadAsync<Site>("sites/" + _locale.TwoLetterISOLanguageName, token);

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

                _session.StoreAsync(localizedPage, string.Join("/", page.Id, _locale.TwoLetterISOLanguageName), token);

                _session.StoreAsync(Entity, string.Join("/", page.Id, _locale.TwoLetterISOLanguageName, "content"), token);

            }, token);
        }
    }
}