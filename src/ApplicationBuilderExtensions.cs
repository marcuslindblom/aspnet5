using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
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

            app.UseRequestLocalization(options);

            var documentStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();

            IndexCreation.CreateIndexes(typeof(Startup).Assembly, documentStore);

            app.UseMvc(routes =>
            {
                //routes.DefaultHandler = new DefaultRouter(
                    //routes.DefaultHandler,
                    //new DefaultRouteResolver(
                    //    new RouteResolverTrie(documentStore),
                    //    app.ApplicationServices.GetService<IControllerMapper>()),
                    //new DefaultVirtualPathResolver(
                    //    new RouteResolverTrie(documentStore),
                    //    app.ApplicationServices.GetService<IControllerMapper>()),
                    //options.DefaultRequestCulture);

                routes.Routes.Insert(0, new DefaultRouter(
                    routes.DefaultHandler,
                    new DefaultRouteResolver(
                        new RouteResolverTrie(documentStore),
                        app.ApplicationServices.GetService<IControllerMapper>()),
                    new DefaultVirtualPathResolver(
                        new RouteResolverTrie(documentStore),
                        app.ApplicationServices.GetService<IControllerMapper>()),
                    options.DefaultRequestCulture));

                //routes.MapRoute(
                //    name: "default_localization",
                //    template: "{culture?}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "AreasRoute",
                    template: "{area}/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });

            Task.Run(() => app.UseSampleData(options)).Wait();

        }

        public static async void UseSampleData(this IApplicationBuilder app, RequestLocalizationOptions options)
        {
            var documentStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();

            var config = documentStore.OpenSession().Advanced.Exists("brickpile/configuration");

            if (config) return;

            using (var session = documentStore.OpenAsyncSession())
            {
                foreach (var culture in options.SupportedCultures)
                {
                    await session.StoreAsync(new Site(culture.DisplayName, culture.TwoLetterISOLanguageName, options.DefaultRequestCulture.Culture.LCID == culture.LCID));
                }
                await session.StoreAsync(new Configuration { Id = "brickpile/configuration" });
                await session.SaveChangesAsync();
            }

            using (var session = documentStore.OpenAsyncSession())
            {
                // Create a page with default language
                await session
                    .LocalizeFor(new RequestCulture(new CultureInfo("en")))
                    .ForModel(new Home { Heading = "In english" })
                    .ForUrl("/")
                    .StoreAsync(new Page { Name = "Home", PublishedDate = DateTime.Now });

                await session
                    .LocalizeFor(new RequestCulture(new CultureInfo("en")))
                    .ForModel(new About { Heading = ".NET Core ? RavenDB" })
                    .ForUrl("/about")
                    .StoreAsync(new Page { Name = "About" } );

                await session.SaveChangesAsync();
            }

            using (var session = documentStore.OpenAsyncSession())
            {
                // Create a new language for a page
                var home = await session.LoadAsync<Page>("pages/1");

                await session
                    //.For(home)
                    .LocalizeFor(home, new RequestCulture(new CultureInfo("sv")))
                    .ForModel(new Home { Heading = "På svenska" })
                    .ForUrl("/")
                    .StoreAsync(new Page { Name = "Hem", PublishedDate = DateTime.Now });

                var about = await session.LoadAsync<Page>("pages/2");

                await session
                    .LocalizeFor(about, new RequestCulture(new CultureInfo("sv")))
                    .ForModel(new About { Heading = "Om oss" } )
                    .ForUrl("/om-oss")
                    .StoreAsync(new Page { Name = "Om oss" });

                await session
                    .LocalizeFor(new RequestCulture(new CultureInfo("sv")))
                    .ForModel(new About {Heading = "En sida till"})
                    .ForUrl("/om-oss/en-sida-till")
                    .StoreAsync(new Page {Name = "En sida till"});

                await session.SaveChangesAsync();
            }

        }

        class Configuration
        {
            public string Id { get; set; } 
        }
    }
}