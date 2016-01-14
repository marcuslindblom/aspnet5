using System.Globalization;
using Raven.Client;

namespace src.Localization
{
    public class LocalizationOperation : ILocalizationOperation
    {
        private readonly IDocumentSession _session;
        private readonly CultureInfo _locale;
        private readonly CultureInfo _fallbackLocale;

        public LocalizationOperation(IDocumentSession session, CultureInfo locale, CultureInfo fallbackLocale)
        {
            _session = session;
            _locale = locale;
            _fallbackLocale = fallbackLocale;
        }

        public string Key { get; set; }
        public object Entity { get; set; }
        public Page Page { get; set; }
        public T Load<T>(string id)
        {
            return _session.Load<LocalizationTransformer, T>(id,
                configuration =>
                {
                    configuration.AddTransformerParameter("Locale", _locale.TwoLetterISOLanguageName);
                    configuration.AddTransformerParameter("FallbackLocale", _fallbackLocale?.TwoLetterISOLanguageName);
                });
        }
    }
}