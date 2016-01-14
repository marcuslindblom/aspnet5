using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace brics.Areas.Brics.Controllers
{
    [Area("brics")]
    //[Authorize]
    public class DashboardController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
