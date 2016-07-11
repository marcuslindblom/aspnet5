using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Raven.Client;

namespace src.Mvc.ModelBinding
{
    public class DefaultModelBinderProvider : IModelBinderProvider
    {
        private readonly IDocumentStore _documentStore;

        public DefaultModelBinderProvider(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }
            return new DefaultModelBinder(_documentStore);
        }
    }
}
