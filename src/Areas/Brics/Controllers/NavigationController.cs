using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Raven.Client;
using Raven.Client.Documents;
using src.Localization;
using src.Routing.Trie;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace src.Areas.Brics.Controllers
{
    [Route("api/[controller]")]
    [Area("brics")]
    public class NavigationController : Controller
    {
        private readonly IDocumentStore _documentStore;
        private readonly IRouteResolverTrie _trieResolver;
        private readonly IHttpContextAccessor _context;
        private RequestCulture CurrentRequestCulture => _context.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture;

        public NavigationController(IDocumentStore documentStore, IRouteResolverTrie trieResolver, IHttpContextAccessor context)
        {
            _documentStore = documentStore;
            _trieResolver = trieResolver;
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, string url)
        {
            var trie = await _trieResolver.LoadTrieAsync(new RequestCulture(CultureInfo.CurrentCulture));

            TrieNode node;
            if (trie.TryGetNode(url, out node))
            {
                using (var session = _documentStore.OpenAsyncSession())
                {                    
                    var ids = trie.ChildrenOf(url).Select(x => x.Value.PageId);
                    var pages = await session.LocalizeFor(CurrentRequestCulture).LoadAsync(ids);
                    return PartialView("_Navigation", pages);
                }
            }

            return NotFound();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
