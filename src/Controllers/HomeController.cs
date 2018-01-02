using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raven.Client;
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
        public HomeController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<IActionResult> Index(Page currentPage, Home model)
        {
            using(var session = _documentStore.OpenAsyncSession()) {
                var postfix = $"/en";
                var query = from page in session.Query<Page>()
                                                    where page.Id.In(new[] { "pages/2-A" })
                            let localizedDocument = RavenQuery.Load<Page>(page.Id + postfix)
                            select new Page
                            {
                                Id = page.Id,
                                Name = localizedDocument.Name,
                            };

                var result = await query.ToListAsync();
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
