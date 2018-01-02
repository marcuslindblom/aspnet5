using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace src.Localization
{
    public class RouteRequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var request = httpContext.Request;
            if (!request.Path.HasValue)
            {
                return Task.FromResult((ProviderCultureResult)null);
            }

            var cultureValue = Regex.Match(
                request.Path.Value,
                @"^/([a-z]{2})(?:$|/)",
                RegexOptions.IgnoreCase);

            if (cultureValue.Success)
            {

                var culture = CultureInfo.CurrentCulture;
                var uiCulture = CultureInfo.CurrentUICulture;

                if (culture == null || uiCulture == null)
                {
                    return Task.FromResult((ProviderCultureResult)null);
                }

                if (culture.Name == Options.DefaultRequestCulture.Culture.TwoLetterISOLanguageName)
                //if(culture.Name == new CultureInfo("sv").TwoLetterISOLanguageName)
                {
                    if (httpContext.Request.Path.Equals(new PathString("/" + culture.TwoLetterISOLanguageName)))
                    {
                        httpContext.Response.Redirect("/", true);
                    }
                    else
                    {
                        var remaining = httpContext.Request.Path.Value.Substring(3);
                        httpContext.Response.Redirect(remaining, true);
                    }
                }

                var requestCulture = new ProviderCultureResult(culture.Name, uiCulture.Name);

                return Task.FromResult(requestCulture);
            }
            else
            {
                var requestCulture = new ProviderCultureResult(Options.DefaultRequestCulture.Culture.TwoLetterISOLanguageName, Options.DefaultRequestCulture.UICulture.TwoLetterISOLanguageName);
                //var requestCulture = new ProviderCultureResult(new CultureInfo("sv").TwoLetterISOLanguageName, new CultureInfo("sv").TwoLetterISOLanguageName);

                return Task.FromResult(requestCulture);
            }
        }
    }
}