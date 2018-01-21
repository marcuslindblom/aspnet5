using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using src.Localization;
using src.Models;

namespace src.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IDocumentStore _documentStore;
        private readonly IOptions<RequestLocalizationOptions> _requestLocalizationOptions;
        public HomeController(IDocumentStore documentStore, IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            _documentStore = documentStore;
            _requestLocalizationOptions = requestLocalizationOptions;
        }

        public async Task<IActionResult> Index(Page currentPage, Home model)
        {
            using(var session = _documentStore.OpenAsyncSession()) {
                // var postfix = $"/en";
                // var query = from page in session.Query<Page>()
                //             where page.Id.In(new[] { "pages/65-A" })
                //             let localizedDocument = RavenQuery.Load<Page>(page.Id + postfix)
                //             let metadata = RavenQuery.Metadata(page)
                //             select new
                //             {
                //                 Id = page.Id,
                //                 Name = localizedDocument.Name,
                //                 Metadata = metadata
                //             };

                // var result = await query.ToListAsync();
            }
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
