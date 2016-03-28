using System.Globalization;
using Raven.Client;

namespace src.Localization
{
    public class LocalizationOperation : ILocalizationOperation
    {
        private readonly IDocumentSession _session;
        private readonly CultureInfo _locale;

        public LocalizationOperation(IDocumentSession session, CultureInfo locale)
        {
            _session = session;
            _locale = locale;
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
                });
        }
    }
}