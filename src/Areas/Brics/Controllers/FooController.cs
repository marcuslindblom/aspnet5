using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using src.Models;
using brics.Mvc.ModelBinding;
using Raven.Client;
using src.Localization;
using System.Globalization;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace src.Areas.Brics.Controllers
{
    [Route("api/[controller]")]
    [Area("brics")]
    public class FooController : Controller
    {
        private IDocumentStore _documentStore;

        public FooController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, string propertyName)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var model = await session.LoadAsync<dynamic>("pages/2/sv/content");

                return PartialView("_Editor", new FooViewModel
                {
                    PropertyName = propertyName,
                    Model = model
                });
            }
        }

        // POST api/values
        [HttpPost]
        public async Task Post()
        {
            dynamic model;
            using (var session = _documentStore.OpenAsyncSession())
            {
                model = await session.LoadAsync<dynamic>("pages/2/sv/content");
            }

            if (await TryUpdateModelAsync(model, "Model"))
            {
                using (var session = _documentStore.OpenAsyncSession())
                {
                    await session.StoreAsync(model, "pages/2/sv/content");
                    await session.SaveChangesAsync();
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(string id, [ModelBinder(BinderType = typeof(FromContentType))] dynamic model)
        {
            var identity = id;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class FooViewModel
    {
        public string PropertyName { get; set; }
        public dynamic Model { get; set; }
    }
}
