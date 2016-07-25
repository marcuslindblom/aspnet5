using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Raven.Abstractions.Extensions;
using Raven.Client;
using src.Areas.Brics.Models;
using src.Localization;
using src.Mvc;
using src.Routing.Trie;

namespace src.Areas.Brics.Controllers
{
    [Route("api/[controller]")]
    [Area("brics")]
    public class SettingsController : Controller
    {
        private readonly IDocumentStore _documentStore;
        private readonly IRouteResolverTrie _trieResolver;

        public SettingsController(IDocumentStore documentStore, IRouteResolverTrie trieResolver)
        {
            _documentStore = documentStore;
            _trieResolver = trieResolver;
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
                    var page = await session.LocalizeFor(CultureInfo.CurrentCulture).LoadAsync<Page>(node.PageId);
                    var viewModel = new SettingsViewModel
                    {
                        CurrentPage = page,
                        Acl =
                            new SelectList(
                                new[]
                                {
                                    AccessControl.Anonymous,
                                    AccessControl.Authenticated,
                                    AccessControl.Administrators
                                },
                                page.Acl),
                        Url = TrimLastSegment(url),
                        Slug = url.Split(new char[] { '/' }).LastOrDefault(),
                        PublishedDate = page.PublishedDate?.ToString("dd MMM yy @ HH:mm") ?? string.Empty
                    };
                    return PartialView("_Settings", viewModel);
                }
            }
            return NotFound();
        }
        // POST api/values
        public async Task Post([FromForm] SettingsViewModel model, [FromForm] string url)
        {
            if (ModelState.IsValid)
            {
                using (var session = _documentStore.OpenAsyncSession())
                {
                    var p = await session.LocalizeFor(CultureInfo.CurrentCulture).LoadAsync<Page>(model.CurrentPage.Id);

                    if (await TryUpdateModelAsync(p, "CurrentPage"))
                    {
                        await session
                            .LocalizeFor(model.CurrentPage, CultureInfo.CurrentCulture)
                            .ForUrl(url)
                            .StoreAsync(p);

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

        private string TrimLastSegment(string url)
        {
            var segments = url.Split(new char[] {'/'});
            segments = segments.Take(segments.Length - 1).ToArray();
            return string.Join("/", segments);
        }
    }
}