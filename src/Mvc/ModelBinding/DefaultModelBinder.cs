using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Raven.Client;
using Raven.Abstractions.Extensions;

namespace src.Mvc.ModelBinding
{
    public class DefaultModelBinder : IModelBinder
    {
        readonly IDocumentStore _documentStore;
        public DefaultModelBinder(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var binderType = ResolveBinderType(bindingContext);
            if (binderType == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            var binder = (IModelBinder)Activator.CreateInstance(binderType, _documentStore);

            var collectionBinder = binder as ICollectionModelBinder;
            if (collectionBinder != null &&
                bindingContext.Model == null &&
                !collectionBinder.CanCreateInstance(bindingContext.ModelType))
            {
                // Able to resolve a binder type but need a new model instance and that binder cannot create it.
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return BindModelAsync(bindingContext, binder);
        }

        private async Task BindModelAsync(ModelBindingContext bindingContext, IModelBinder binder)
        {
            await binder.BindModelAsync(bindingContext);
            
/*            bindingContext.Result = await binder.BindModelAsync(bindingContext);
            var modelBindingResult = result != ModelBindingResult.NoResult
                ? result
                : ModelBindingResult.NoResult;

            return bindingContext;*/
        }

        private static Type ResolveBinderType(ModelBindingContext context)
        {
            var modelType = context.ModelType;

            return GetPageModelBinder(modelType) ??
                   GetModelBinder(modelType);
        }

        private static Type GetPageModelBinder(Type modelType)
        {
            if (modelType == typeof(Page))
            {
                return typeof(PageModelBinder);
            }
            return null;
        }

        private static Type GetModelBinder(Type modelType)
        {
            if (modelType.IsDefined(typeof(ViewModelAttribute), false))
            {
                return typeof(ModelModelBinder);
            }
            return null;
        }
    }
}
