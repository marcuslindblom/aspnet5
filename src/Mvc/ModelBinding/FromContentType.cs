using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace brics.Mvc.ModelBinding
{
    public class FromContentType : IModelBinder
    {
        const string ContentType = "ContentType";

        public async Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {            
            IFormCollection form = await bindingContext.OperationBindingContext.HttpContext.Request.ReadFormAsync();
            var contentType = form[ContentType];
            if (string.IsNullOrEmpty(contentType))
            {
                return ModelBindingResult.NoResult;
            }
            var model = Activator.CreateInstance(Type.GetType(contentType));

            return await ModelBindingResult.SuccessAsync(bindingContext.ModelName, model);
        }
    }
}
