using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;
using Raven.Client;

namespace src.Mvc.ModelBinding
{
    public class DefaultModelBinder : IModelBinder
    {
        readonly IDocumentStore _documentStore;
        public DefaultModelBinder(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            var binderType = ResolveBinderType(bindingContext);
            if (binderType == null)
            {
                return ModelBindingResult.NoResultAsync;
            }

            var binder = (IModelBinder)Activator.CreateInstance(binderType, _documentStore);

            var collectionBinder = binder as ICollectionModelBinder;
            if (collectionBinder != null &&
                bindingContext.Model == null &&
                !collectionBinder.CanCreateInstance(bindingContext.ModelType))
            {
                // Able to resolve a binder type but need a new model instance and that binder cannot create it.
                return ModelBindingResult.NoResultAsync;
            }

            return BindModelAsync(bindingContext, binder);
        }

        private async Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext, IModelBinder binder)
        {
            var result = await binder.BindModelAsync(bindingContext);
            var modelBindingResult = result != ModelBindingResult.NoResult
                ? result
                : ModelBindingResult.NoResult;

            return modelBindingResult;
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
