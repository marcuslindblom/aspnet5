//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.TagHelpers;
//using Microsoft.AspNetCore.Razor.TagHelpers;
//using Microsoft.Extensions.DependencyInjection;

//namespace src.TagHelpers
//{
//    [HtmlTargetElement("a", Attributes = ActionAttributeName)]
//    [HtmlTargetElement("a", Attributes = ControllerAttributeName)]
//    [HtmlTargetElement("a", Attributes = RouteValuesPrefix + "*")]
//    public class AnchorTagHelper : TagHelper
//    {
//        private readonly IUrlHelper _urlHelper;

//        private const string ActionAttributeName = "asp-action";
//        private const string ControllerAttributeName = "asp-controller";
//        private const string RouteValuesPrefix = "asp-route-";

//        public AnchorTagHelper(IHttpContextAccessor contextAccessor)
//        {
//            _urlHelper = contextAccessor.HttpContext.RequestServices.GetService<IUrlHelper>();
//        }

//        [HtmlAttributeName(ActionAttributeName)]
//        public string Action { get; set; }

//        [HtmlAttributeName(ControllerAttributeName)]
//        public string Controller { get; set; }

//        [HtmlAttributeName(DictionaryAttributePrefix = RouteValuesPrefix)]
//        public IDictionary<string, object> RouteValues { get; set; } =
//            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

//        public override void Process(TagHelperContext context, TagHelperOutput output)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            if (output == null)
//            {
//                throw new ArgumentNullException(nameof(output));
//            }

//            // Convert from Dictionary<string, string> to Dictionary<string, object>.
//            var routeValues = RouteValues.ToDictionary(
//                kvp => kvp.Key,
//                kvp => (object)kvp.Value,
//                StringComparer.OrdinalIgnoreCase);

//            var tagBuilder = new TagBuilder("a");
//            tagBuilder.MergeAttribute("href", _urlHelper.Action(Action, Controller, routeValues));

//            output.MergeAttributes(tagBuilder);
//        }
//    }
//}