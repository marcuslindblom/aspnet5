using System.Globalization;
using Raven.Client;
using Raven.Client.Documents.Session;
using src.Mvc;
using src.Security;

namespace src.Localization
{
    public static class DocumentSessionExtensions
    {
        public static ILocalizationOperation LocalizeFor(this IDocumentSession session, CultureInfo locale)
        {
            return new LocalizationOperation(session, locale);
        }

        public static ILocalizationOperation LocalizeFor(this IDocumentSession session, Page page, CultureInfo locale)
        {
            return new LocalizationOperation(session, locale) { Page = page };
        }

        public static IAsyncLocalizationOperation LocalizeFor(this IAsyncDocumentSession session, CultureInfo locale)
        {
            return new AsyncLocalizationOperation(session, locale);
        }

        public static IAsyncLocalizationOperation LocalizeFor(this IAsyncDocumentSession session, Page page, CultureInfo locale)
        {
            return new AsyncLocalizationOperation(session, locale) { Page = page };
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

        public static IAsyncAclOperation WithAcl(this IAsyncDocumentSession session, AccessControl acl = AccessControl.Anonymous)
        {
            return new AsyncAclOperation(session, acl);
        }
    }
}
