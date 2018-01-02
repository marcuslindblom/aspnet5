using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Raven.Client.Documents;
using src.Localization;
using src.Routing;
using src.Routing.Trie;

namespace src.Mvc.ModelBinding
{
    public class PageModelBinder : IModelBinder
    {
        readonly IDocumentStore _documentStore;
        public PageModelBinder(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            object value;
            if (!bindingContext.HttpContext.Items.TryGetValue(DefaultRouter.CurrentNodeKey, out value))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            TrieNode node = value as TrieNode;
            if (node == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var requestCultureFeature = bindingContext.HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;

            using (var session = _documentStore.OpenAsyncSession())
            {
                var localizedPage = await session.LocalizeFor(requestCulture.Culture).LoadAsync(node.PageId);
                bindingContext.HttpContext.Items[DefaultRouter.CurrentPageKey] = localizedPage;
                bindingContext.Result = ModelBindingResult.Success(localizedPage);
            }            
        }
    }
}
