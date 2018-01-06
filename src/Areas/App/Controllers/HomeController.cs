using Microsoft.AspNetCore.Mvc;

namespace src.Areas.Brics.Controllers
{
    [Area("App")]
    public class HomeController : Controller
    {
      public HomeController() {

      }

      public IActionResult Index() {
        return View();
      }
    }
}