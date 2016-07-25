using System.Linq;
using Raven.Client.Indexes;

namespace src.Localization
{
    public class LocalizationTransformer : AbstractTransformerCreationTask<Page>
    {
        public LocalizationTransformer()
        {
            TransformResults = pages => from page in pages
                //let fallbackLocale = ParameterOrDefault("FallbackLocale", null).Value<string>()
                let locale = ParameterOrDefault("Locale", null).Value<string>()
                let localizedDocument = LoadDocument<Page>(page.Id + "/" + locale)
                //let localizedDocument = localizedDocuments.FirstOrDefault()
                //let fallbackDocument = localizedDocuments.LastOrDefault()
                select new
                {
                    page.Id,
                    Name = localizedDocument.Name ?? page.Name,
                    PublishedDate = localizedDocument.PublishedDate,
                    page.Acl,
                    Metadata = localizedDocument.Metadata ?? page.Metadata
                };
        }
    }
}