using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Localization;
using Microsoft.Extensions.DependencyInjection;
using src.Localization;

namespace src
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBrickPile(this IServiceCollection services)
        {
            services.ConfigureRouting(routeOptions =>
            {
                routeOptions.AppendTrailingSlash = true;
                routeOptions.LowercaseUrls = true;
            });
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static void UseBrickPile(this IApplicationBuilder app)
        {
            
        }

        public static void UseBrickPile(this IApplicationBuilder app, Action<RequestLocalizationOptions> configureOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new RequestLocalizationOptions();
            configureOptions(options);

            options.RequestCultureProviders.Insert(0, new RouteRequestCultureProvider
            {
                Options = options
            });

            app.UseRequestLocalization(options);
        }
    }
}
