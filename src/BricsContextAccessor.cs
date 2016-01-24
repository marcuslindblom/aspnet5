using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Raven.Client;
using src.Localization;
using src.Routing;
using src.Routing.Trie;

namespace src
{
    public class BricsContextAccessor : IBricsContextAccessor
    {
        private readonly IHttpContextAccessor _context;
        private readonly IDocumentStore _documentStore;
        private TrieNode _currentNode;
        private Page _currentPage;
        public BricsContextAccessor(IHttpContextAccessor context, IDocumentStore documentStore)
        {
            _context = context;
            _documentStore = documentStore;
        }

        public TrieNode CurrentNode => _currentNode ?? (_currentNode = _context.HttpContext.Items[DefaultRouter.CurrentNodeKey] as TrieNode);

        private RequestCulture CurrentRequestCulture => _context.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture;

        public Page CurrentPage
        {
            get
            {
                if (CurrentNode == null)
                {
                    return null;
                }
                if (_currentPage == null)
                {
                    _currentPage = _context.HttpContext.Items[DefaultRouter.CurrentPageKey] as Page;
                    if (_currentPage == null)
                    {
                        using (var session = _documentStore.OpenSession())
                        {
                            _currentPage = session.LocalizeFor(CurrentRequestCulture.Culture).Load<Page>(CurrentNode.PageId);
                        }
                    }                    
                }
                return _currentPage;
            }
        }
            
    }
}