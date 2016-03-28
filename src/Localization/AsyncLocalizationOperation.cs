using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client;
using src.Routing.Trie;

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

        public Task<T> LoadAsync<T>(string id, CancellationToken token = default(CancellationToken))
        {
            return _session.LoadAsync<LocalizationTransformer, T>(id,
                configuration =>
                {
                    configuration.AddTransformerParameter("Locale", _locale.TwoLetterISOLanguageName);
                }, token);
        }

        public Task<T[]> LoadAsync<T>(IEnumerable<string> ids, CancellationToken token = default(CancellationToken))
        {
            return _session.LoadAsync<LocalizationTransformer, T>(ids,
                configuration =>
                {
                    configuration.AddTransformerParameter("Locale", _locale.TwoLetterISOLanguageName);
                }, token);
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