using System.Globalization;
using Raven.Client.Documents.Session;
using System.Linq;
using Raven.Client.Documents.Queries;
using Microsoft.AspNetCore.Localization;

namespace src.Localization
{
    public class LocalizationOperation : ILocalizationOperation
    {
        private readonly IDocumentSession _session;
        private readonly RequestCulture _requestCulture;

        public LocalizationOperation(IDocumentSession session, RequestCulture requestCulture)
        {
            _session = session;
            _requestCulture = requestCulture;
        }

        public string Key { get; set; }
        public object Entity { get; set; }
        public Page Page { get; set; }
        public Page Load(string id)
        {
            var postfix = $"/{_requestCulture.Culture.TwoLetterISOLanguageName}";
            var query = from page in _session.Query<Page>()
                        where page.Id == id
                        let localizedDocument = RavenQuery.Load<Page>(page.Id + postfix)
                        select new Page
                        {
                            Id = page.Id,
                            Name = localizedDocument.Name,
                            Metadata = localizedDocument.Metadata
                        };

            return query.FirstOrDefault();
        }
    }
}