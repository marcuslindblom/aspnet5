using System.Linq;
using Raven.Client.Indexes;

namespace src.Localization
{
    public class LocalizationTransformer : AbstractTransformerCreationTask<Page>
    {
        public LocalizationTransformer()
        {
            TransformResults = pages => from page in pages
                let fallbackLocale = ParameterOrDefault("FallbackLocale", null).Value<string>()
                let locale = ParameterOrDefault("Locale", null).Value<string>()
                let localizedDocuments = LoadDocument<Page>(new[] { page.Id + "/" + locale, page.Id + "/" + fallbackLocale })
                let localizedDocument = localizedDocuments.FirstOrDefault()
                let fallbackDocument = localizedDocuments.LastOrDefault()
                select new
                {
                    page.Id,
                    Name = localizedDocument.Name ?? fallbackDocument.Name,
                    Metadata = localizedDocument.Metadata ?? fallbackDocument.Metadata
                };
        }
    }
}