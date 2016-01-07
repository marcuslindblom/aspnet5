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

        public IActionResult Index(Home home)
        {
            return View(home);
        }

        //public IActionResult About(Home home)
        //{
        //    ViewData["Title"] = home?.Heading;
        //    ViewData["Message"] = "Your application description page.";

        //    return View();
        //}

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
