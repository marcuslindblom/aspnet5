using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Raven.Client;
using src.Localization;
using src.Models;
using src.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace src.Areas.Brics.Controllers
{
    [Area("brics")]
    [Route("[area]/[controller]")]
    public class EditorController : Controller
    {
        private readonly IDocumentStore _store;

        public EditorController(IDocumentStore store)
        {
            _store = store;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            //using (var session = _store.OpenAsyncSession())
            //{

            //    var viewModel = new EditorViewModel();
            //    var page = await session.LocalizeFor(CultureInfo.CurrentCulture).LoadAsync<Page>("pages/" + id);
            //    viewModel.CurrentPage = page;
            //    viewModel.Acl = 
            //        new SelectList(
            //            new[]
            //            {
            //                AccessControl.Anonymous,
            //                AccessControl.Authenticated,
            //                AccessControl.Administrators
            //            },
            //            page.Acl);
            //    return View(viewModel);
            //}
            return View();
        }
    }

    public class EditorViewModel
    {
        public Page CurrentPage { get; set; }
        public SelectList Acl { get; set; }
    }
}
