using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using src.Models;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Mvc.ViewFeatures.Internal;

namespace src.TagHelpers
{
    [HtmlTargetElement("*", Attributes = AttributeName)]
    public class ComponentTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBricsContextAccessor _bricsContext;
        private IUrlHelper _urlHelper;
        private readonly IHtmlHelper _htmlHelper
            ;
        private const string AttributeName = "asp-module";

        public ComponentTagHelper(IHttpContextAccessor contextAccessor, IBricsContextAccessor bricsContext)
        {
            _contextAccessor = contextAccessor;
            _bricsContext = bricsContext;
            _urlHelper = contextAccessor.HttpContext.RequestServices.GetService<IUrlHelper>();
            _htmlHelper = contextAccessor.HttpContext.RequestServices.GetService<IHtmlHelper>();
        }

        [HtmlAttributeName(AttributeName)]
        public string Module { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            (_htmlHelper as ICanHasViewContext)?.Contextualize(ViewContext);

            var content = await output.GetChildContentAsync();

            output.PreContent.SetHtmlContent(
                "<script type=\"text/x-config\">{\"itemsPerPage\":10,\"root\":\"/home\"}</script>");

            //if (!content.IsEmpty && !content.IsWhiteSpace)
            //{
            //    return;
            //}

            output.Attributes["id"] = "_" + context.UniqueId;
            output.Attributes["data-module"] = "widget";

            //var form = _htmlHelper.Editor(Module);

            //var tagHelperContent = output.Content.SetContent(form);

            //output.PreContent.SetHtmlContent(
            //    @"<script type='text/x-config'>{ 'itemsPerPage':10,'root':'/home'}</script>");

            await _htmlHelper.RenderPartialAsync("~/Views/ComponentTagHelper/_Template.cshtml", _htmlHelper.ViewData.Model);

            //output.PostContent.SetHtmlContent(template.ToString());

            //output.PostContent.SetHtmlContent(_htmlHelper.Editor("name").ToString());

            //output.PostContent.SetHtmlContent(@"
                //< template class='widget-template'>
                //    <style>
                //        div { text-align: center; }
                //        :host.button {
                //            font-size: 23px;
                //            font-weight: 100;
                //            border: none;
                //            background: transparent;
                //            opacity: 0;
                //            transition: opacity .25s ease-out;
                //            cursor: pointer;
                //            outline: none;
                //            color:#999;
                //        }
                //        :host(:hover) .button {
                //            opacity: 1;
                //        }
                //        .brics {
                //            transition: height .10s ease-out;
                //            height: 0;
                //            background: #2e3332;
                //            visibility: hidden;
                //            position: absolute;
                //            left: 0;
                //            width: 100vw;
                //            padding: 1.5em 0 0;
                //        }
                //        .brics li { display: inline-block; }
                //        :host.open { height: 120px; }
                //        :host.open.brics {
                //            visibility: visible;
                //            height: 70px;
                //        }
                //    </style>
                //    <div data-module='widget'>
                //        <script type = 'text/x-config' >{ 'itemsPerPage': '10' }</script>
                //        <button class='button' data-type='toggle-btn'>+</button>
                //        <div class='brics'>
                //            <ul>
                //                <li data-module='foo'><button data-type='btn'>Enbart rubrik</button></li>
                //                <li data-module= 'foo' >< button data-type= 'btn' > Rubrik och ingress</button></li>
                //            </ul>
                //        </div>
                //    </div>
                //</template>
            //");
        }
    }
}
