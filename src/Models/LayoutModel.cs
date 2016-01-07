using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Raven.Client;

namespace src.Models
{
    public class LayoutModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LayoutModel(IDocumentStore documentStore, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            Initialize(documentStore);
        }

        public void Initialize(IDocumentStore documentStore)
        {
            //using (var session = documentStore.OpenSession())
            //{
            //    var start = session.Load<Home>("pages/1/content");
            //    Title = start.Heading;
            //}
        }

        public string MetaTitle => "The meta title from the page";

        public string MetaDescription => "The meta description from the page";

        public string CanonicalUrl => "https://aspnet5rc2.azurewebsites.net/";

        public string LanguageName
        {
            get
            {
                var requestCultureFeature = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
                var requestCulture = requestCultureFeature.RequestCulture;
                return requestCulture.Culture.TwoLetterISOLanguageName;
            }
        }
    }
}
