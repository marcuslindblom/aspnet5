using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<RequestLocalizationOptions> _requestLocalizationOptions;

        public DashboardController(IDocumentStore store, IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            _store = store;
            _requestLocalizationOptions = requestLocalizationOptions;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(int id)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var page = await session.LocalizeFor(_requestLocalizationOptions.Value.DefaultRequestCulture).LoadAsync("pages/" + id);
                return View(page);
            }
        }
    }
}
