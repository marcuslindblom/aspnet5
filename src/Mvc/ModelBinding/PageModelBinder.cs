using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Mvc.ModelBinding;
using Raven.Client;
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
        public async Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            object value;
            if (!bindingContext.OperationBindingContext.HttpContext.Items.TryGetValue(DefaultRouter.CurrentNodeKey, out value))
            {
                return await ModelBindingResult.NoResultAsync;
            }
            TrieNode node = value as TrieNode;
            if (node == null)
            {
                return await ModelBindingResult.NoResultAsync;
            }

            var requestCultureFeature = bindingContext.OperationBindingContext.HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;

            using (var session = _documentStore.OpenAsyncSession())
            {
                return
                    await
                        ModelBindingResult.SuccessAsync(bindingContext.ModelName,
                            await session.LocalizeFor(requestCulture.Culture).LoadAsync<Page>(node.PageId));
            }
        }
    }
}
