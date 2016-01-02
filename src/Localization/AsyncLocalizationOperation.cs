using System.Globalization;
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
        private readonly CultureInfo _fallbackLocale;

        public string Key { get; set; }

        public object Entity { get; set; }

        public Page Page { get; set; }

        public AsyncLocalizationOperation(IAsyncDocumentSession session, CultureInfo locale, CultureInfo fallbackLocale)
        {
            _session = session;
            _locale = locale;
            _fallbackLocale = fallbackLocale;
        }

        public Task<T> LoadAsync<T>(string id, CancellationToken token = default(CancellationToken))
        {
            return _session.LoadAsync<LocalizationTransformer, T>(id,
                configuration =>
                {
                    configuration.AddTransformerParameter("Locale", _locale.TwoLetterISOLanguageName);
                    configuration.AddTransformerParameter("FallbackLocale", _fallbackLocale?.TwoLetterISOLanguageName);
                }, token);
        }

        public Task StoreAsync(Page localizedPage, CancellationToken token = default(CancellationToken))
        {
            if (localizedPage == null) return null;

            var page = Page ?? new Page();

            _session.StoreAsync(page, token).Wait(token);

            Task<Site> siteTask     = _session.LoadAsync<Site>("sites/" + _locale.TwoLetterISOLanguageName, token);

            return Task.WhenAll(siteTask).ContinueWith(task =>
            {
                if (!string.IsNullOrEmpty(Key))
                {
                    var trie = siteTask.Result.Trie;

                    if (!trie.ContainsKey(Key))
                    {
                        trie.Add(Key, new TrieNode(page.Id, localizedPage.Name, Entity.GetType().Name));
                    }
                }

                _session.StoreAsync(localizedPage, string.Join("/", page.Id, _locale.TwoLetterISOLanguageName), token);

                _session.StoreAsync(Entity, string.Join("/", page.Id, _locale.TwoLetterISOLanguageName, "content"), token);

            }, token);
        }
    }
}