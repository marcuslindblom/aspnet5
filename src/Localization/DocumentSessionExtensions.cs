using System.Globalization;
using Raven.Client;

namespace src.Localization
{
    public static class DocumentSessionExtensions
    {
        public static IAsyncLocalizationOperation LocalizeFor(this IAsyncDocumentSession session, CultureInfo locale, CultureInfo fallbackLocale = null)
        {
            return new AsyncLocalizationOperation(session, locale, fallbackLocale);
        }

        public static IAsyncLocalizationOperation LocalizeFor(this IAsyncDocumentSession session, Page page, CultureInfo locale, CultureInfo fallbackLocale = null)
        {
            return new AsyncLocalizationOperation(session, locale, fallbackLocale) { Page = page };
        }

        public static IAsyncLocalizationOperation ForUrl(this IAsyncLocalizationOperation session, string key)
        {
            session.Key = key;
            return session;
        }

        public static IAsyncLocalizationOperation ForModel(this IAsyncLocalizationOperation session, object entity)
        {
            session.Entity = entity;
            return session;
        }
    }
}
