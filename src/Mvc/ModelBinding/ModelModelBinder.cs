using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Raven.Client;
using src.Routing;
using src.Routing.Trie;

namespace src.Mvc.ModelBinding
{
    public class ModelModelBinder : IModelBinder
    {
        readonly IDocumentStore _documentStore;

        public ModelModelBinder(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            object value;
            if(!bindingContext.ActionContext.HttpContext.Items.TryGetValue(DefaultRouter.CurrentNodeKey, out value))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                //return await ModelBindingResult.NoResultAsync;
            }
            TrieNode node = value as TrieNode;
            if (node == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                //return await ModelBindingResult.NoResultAsync;
            }

            var requestCultureFeature = bindingContext.ActionContext.HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;

            using (var session = _documentStore.OpenAsyncSession())
            {
                bindingContext.Result = ModelBindingResult.Success(await session.LoadAsync<dynamic>(string.Join("/", node.PageId, requestCulture.Culture.TwoLetterISOLanguageName,"content")));
/*                return await ModelBindingResult.SuccessAsync(bindingContext.FieldName,
                    );*/
            }
        }
    }
}
