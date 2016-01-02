using System;
using System.Globalization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client;
using Raven.Client.Indexes;
using src.Localization;
using src.Models;
using src.Mvc;
using src.Routing;
using src.Routing.Trie;

namespace src
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseBrickPile(this IApplicationBuilder app){}

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

            app.UseRequestLocalization(options, new RequestCulture("en"));

            var documentStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();
            var controllerTypeProvider = app.ApplicationServices.GetRequiredService<IControllerTypeProvider>();
            var accessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

            IndexCreation.CreateIndexes(typeof(Startup).Assembly, documentStore);

            app.UseMvc(routes =>
            {
                //routes.DefaultHandler = new DefaultRouter(
                //    routes.DefaultHandler,
                //    new DefaultRouteResolver(
                //        new RouteResolverTrie(documentStore, accessor),
                //        new ControllerMapper(controllerTypeProvider)),                    
                //    new DefaultVirtualPathResolver(
                //        new RouteResolverTrie(documentStore, accessor),
                //        new ControllerMapper(controllerTypeProvider)));

                //routes.DefaultHandler = new DefaultRouter(
                //    routes.DefaultHandler,
                //    new DefaultRouteResolver(
                //        new RouteResolverTrie(documentStore, requestCultureFeature),
                //        new ControllerMapper(controllerTypeProvider),
                //        requestCultureFeature),
                //    new DefaultVirtualPathResolver(
                //        new RouteResolverTrie(documentStore, requestCultureFeature),
                //        new ControllerMapper(controllerTypeProvider)));

                routes.Routes.Insert(0, new DefaultRouter(
                    routes.DefaultHandler,
                    new DefaultRouteResolver(
                        new RouteResolverTrie(documentStore, accessor),
                        new ControllerMapper(controllerTypeProvider)),
                    new DefaultVirtualPathResolver(
                        new RouteResolverTrie(documentStore, accessor),
                        new ControllerMapper(controllerTypeProvider),
                        accessor)));

                routes.MapRoute(
                    name: "default_localization",
                    template: "{culture?}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });

            app.UseSampleData(options);

        }

        public static async void UseSampleData(this IApplicationBuilder app, RequestLocalizationOptions options)
        {
            var documentStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();

            var config = documentStore.DatabaseCommands.Head("brickpile/configuration");

            if (config != null) return;

            using (var session = documentStore.OpenAsyncSession())
            {
                foreach (var culture in options.SupportedCultures)
                {
                    await session.StoreAsync(new Site(culture.DisplayName, culture, new CultureInfo("en").LCID == culture.LCID));
                    // await session.StoreAsync(new Site(culture.DisplayName, culture, options.DefaultRequestCulture.Culture.LCID == culture.LCID));
                }
                await session.StoreAsync(new Configuration { Id = "brickpile/configuration" });
                await session.SaveChangesAsync();
            }

            using (var session = documentStore.OpenAsyncSession())
            {
                // Create a page with default language

                await session
                    .LocalizeFor(new CultureInfo("en"))
                    .ForModel(new Home { Heading = "In english" })
                    .ForUrl("/")
                    .StoreAsync(new Page { Name = "Home" });

                await session
                    .LocalizeFor(new CultureInfo("en"))
                    .ForModel(new Home { Heading = "About this yo" })
                    .ForUrl("/about")
                    .StoreAsync(new Page { Name = "About" });

                await session
                    .LocalizeFor(new CultureInfo("en"))
                    .ForModel(new Home { Heading = "Contact this" })
                    .ForUrl("/contact")
                    .StoreAsync(new Page { Name = "Contact" });

                await session.SaveChangesAsync();

            }

            using (var session = documentStore.OpenAsyncSession())
            {
                // Create a new language for a page

                var home = await session.LoadAsync<Page>("pages/1");

                await session
                    .LocalizeFor(home, new CultureInfo("sv"))
                    .ForModel(new Home { Heading = "P� svenska" })
                    .ForUrl("/")
                    .StoreAsync(new Page { Name = "Hem" });

                var about = await session.LoadAsync<Page>("pages/2");

                await session
                    .LocalizeFor(about, new CultureInfo("sv"))
                    .ForModel(new Home { Heading = "Om oss yo" })
                    .ForUrl("/om-oss")
                    .StoreAsync(new Page { Name = "Om oss" });

                await session.SaveChangesAsync();

            }

        }

        class Configuration
        {
            public string Id { get; set; } 
        }
    }
}