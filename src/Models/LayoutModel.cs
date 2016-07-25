using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Raven.Client;

namespace src.Models
{
    public class LayoutModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBricsContextAccessor _bricsContextAccessor;

        public LayoutModel(IDocumentStore documentStore, IHttpContextAccessor httpContextAccessor, IBricsContextAccessor bricsContextAccessor)
        {            
            _httpContextAccessor = httpContextAccessor;
            _bricsContextAccessor = bricsContextAccessor;
            Initialize(documentStore);
        }

        public void Initialize(IDocumentStore documentStore)
        {
            Id = "pages/1";
            MetaTitle = _bricsContextAccessor.CurrentPage.Metadata.MetaTitle;
            MetaDescription = _bricsContextAccessor.CurrentPage.Metadata.MetaDescription;
        }

        public string Id { get; set; }

        //public string MetaTitle => "The meta title from the page";
        public string MetaTitle { get; set; }

        //public string MetaDescription => "The meta description from the page";
        public string MetaDescription { get; set; }

        public string CanonicalUrl => "https://aspnet5rc.azurewebsites.net/";

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
