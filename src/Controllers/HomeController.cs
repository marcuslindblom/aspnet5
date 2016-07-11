using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
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
