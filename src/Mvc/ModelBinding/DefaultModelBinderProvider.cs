using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Raven.Client.Documents;

namespace src.Mvc.ModelBinding
{
    public class DefaultModelBinderProvider : IModelBinderProvider
    {
        private IDocumentStore _documentStore;

        public DefaultModelBinderProvider(IDocumentStore documentStore) {
            _documentStore = documentStore;
        }
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return new DefaultModelBinder(_documentStore);
        }
    }
}
