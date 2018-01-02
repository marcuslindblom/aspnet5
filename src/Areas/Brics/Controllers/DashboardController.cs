using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using src.Localization;

namespace src.Areas.Brics.Controllers
{
    [Area("brics")]
    //[Route("[area]/{id?}")]
    //[Authorize]
    public class DashboardController : Controller
    {
        private readonly IDocumentStore _store;

        public DashboardController(IDocumentStore store)
        {
            _store = store;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(int id)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var page = await session.LocalizeFor(CultureInfo.CurrentCulture).LoadAsync("pages/" + id);
                return View(page);
            }
        }
    }
}
