using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using src.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace src.Controllers
{
    [AllowAnonymous]
    public class AboutController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(About model)
        {
            ViewData["Title"] = "Heading";
            ViewData["Message"] = "Your application description page.";
            return View(model);
        }
    }
}
