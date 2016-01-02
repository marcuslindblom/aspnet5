using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace src.Controllers
{
    [AllowAnonymous]
    public class FooController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return Content("Hello World...");
        }
    }
}
