using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Raven.Client;
using Raven.Client.Documents.Session;
using src.Mvc;
using src.Security;

namespace src.Localization
{
    public static class DocumentSessionExtensions
    {
        public static ILocalizationOperation LocalizeFor(this IDocumentSession session, RequestCulture requestCulture)
        {
            return new LocalizationOperation(session, requestCulture);
        }

        public static ILocalizationOperation LocalizeFor(this IDocumentSession session, Page page, RequestCulture requestCulture)
        {
            return new LocalizationOperation(session, requestCulture) { Page = page };
        }

        public static IAsyncLocalizationOperation LocalizeFor(this IAsyncDocumentSession session, RequestCulture requestCulture)
        {
            return new AsyncLocalizationOperation(session, requestCulture);
        }

        public static IAsyncLocalizationOperation LocalizeFor(this IAsyncDocumentSession session, Page page, RequestCulture requestCulture)
        {
            return new AsyncLocalizationOperation(session, requestCulture) { Page = page };
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
