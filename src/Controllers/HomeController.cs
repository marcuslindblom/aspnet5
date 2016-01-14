using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Raven.Client;
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

        public IActionResult Index(Home home, Page currentPage)
        {
            return View(currentPage);
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

        [HttpPost]
        public async Task<IActionResult> Save([FromForm]Page page)
        {
            if (ModelState.IsValid)
            {
                using (var session = _documentStore.OpenAsyncSession())
                {
                    var p = await session.LocalizeFor(new CultureInfo("sv")).LoadAsync<Page>(page.Id);
                    
                    if (await TryUpdateModelAsync(p))
                    {
                        await session.LocalizeFor(p, new CultureInfo("sv")).StoreAsync(p);
                        await session.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
