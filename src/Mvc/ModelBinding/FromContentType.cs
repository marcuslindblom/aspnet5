using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace brics.Mvc.ModelBinding
{
    public class FromContentType : IModelBinder
    {
        const string ContentType = "ContentType";

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            IFormCollection form = await bindingContext.HttpContext.Request.ReadFormAsync();
            var contentType = form[ContentType];
            if (string.IsNullOrEmpty(contentType))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            var model = Activator.CreateInstance(Type.GetType(contentType));

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}
