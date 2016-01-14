using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNet.Mvc;
using Raven.Client;
using src.Localization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace src.Controllers
{
    [Route("api/[controller]")]
    public class PageController : Controller
    {
        private readonly IDocumentStore _documentStore;

        public PageController(IDocumentStore documentStore)
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
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async void Post(Page page)
        {
            if(ModelState.IsValid) {
                using (var session = _documentStore.OpenAsyncSession())
                {
                    var p = await session.LocalizeFor(new CultureInfo("sv")).LoadAsync<Page>(page.Id);

                    if (await TryUpdateModelAsync(p, null))
                    {
                        await session.LocalizeFor(new CultureInfo("sv")).StoreAsync(p);
                        await session.SaveChangesAsync();
                    }
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
