using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
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

            app.UseRequestLocalization(options);

            //app.UseRequestLocalization(options, new RequestCulture("sv"));

            var documentStore = app.ApplicationServices.GetRequiredService<IDocumentStore>();
            var controllerMapper = app.ApplicationServices.GetRequiredService<IControllerMapper>();
            //var controllerTypeProvider = app.ApplicationServices.GetRequiredService<IControllerTypeProvider>();
            //var controllerFeature = app.ServerFeatures.Get<ControllerFeature>();

            //var manager = app.ApplicationServices.GetService<ApplicationPartManager>();

            new LocalizationTransformer().Execute(documentStore);
            //IndexCreation.CreateIndexes(typeof(Startup).Assembly, documentStore);

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

                routes.DefaultHandler = new DefaultRouter(
                    routes.DefaultHandler,
                    new DefaultRouteResolver(
                        new RouteResolverTrie(documentStore),
                        controllerMapper),
                    new DefaultVirtualPathResolver(
                        new RouteResolverTrie(documentStore),
                        controllerMapper),
                    new RequestCulture("sv"));

                //routes.Routes.Insert(0, new DefaultRouter(
                //    routes.DefaultHandler,
                //    new DefaultRouteResolver(
                //        new RouteResolverTrie(documentStore),
                //        new ControllerMapper(new ControllerFeature())),
                //    new DefaultVirtualPathResolver(
                //        new RouteResolverTrie(documentStore),
                //        new ControllerMapper(new ControllerFeature())),
                //    new RequestCulture("sv")));

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
                    await session.StoreAsync(new Site(culture.DisplayName, culture.TwoLetterISOLanguageName, new CultureInfo("en").Name == culture.Name));
                    // await session.StoreAsync(new Site(culture.DisplayName, culture, options.DefaultRequestCulture.Culture.Name == culture.Name));
                }
                await session.StoreAsync(new Configuration { Id = "brickpile/configuration" });
                await session.SaveChangesAsync();
            }

            using (var session = documentStore.OpenAsyncSession())
            {
                // Create a page with default language
                await session
                    .LocalizeFor(new CultureInfo("en"))
                    .ForModel(new Home { Heading = "In english", })
                    .ForUrl("/")
                    .StoreAsync(new Page { Name = "Home", PublishedDate = DateTime.Now, Metadata = new Metadata { MetaDescription = "Meta desc ...", MetaTitle = "Meta title EN ..."} });

                await session
                    .LocalizeFor(new CultureInfo("en"))
                    .ForModel(new About { Heading = "ASP.NET 5 ? RavenDB" })
                    .ForUrl("/about")
                    .StoreAsync(new Page { Name = "About", PublishedDate = DateTime.Now, Metadata = new Metadata { MetaDescription = "Meta desc ...", MetaTitle = "Meta title EN ..." } } );

                await session.SaveChangesAsync();
            }

            using (var session = documentStore.OpenAsyncSession())
            {
                // Create a new language for a page
                var home = await session.LoadAsync<Page>("pages/1");
                await session
                    //.For(home)
                    .LocalizeFor(home, new CultureInfo("sv"))
                    .ForModel(new Home { Heading = "P� svenska" })
                    .ForUrl("/")
                    .StoreAsync(new Page { Name = "Hem", PublishedDate = DateTime.Now, Metadata = new Metadata { MetaDescription = "Meta desc ...", MetaTitle = "Meta title SV ..." } });

                var about = await session.LoadAsync<Page>("pages/2");

                await session
                    .LocalizeFor(about, new CultureInfo("sv"))
                    .ForModel(new About { Heading = "Om oss" } )
                    .ForUrl("/om-oss")
                    .StoreAsync(new Page { Name = "Om oss", PublishedDate = DateTime.Now, Metadata = new Metadata { MetaDescription = "Meta desc ...", MetaTitle = "Meta title SV ..." } });

                await session
                    .LocalizeFor(new CultureInfo("sv"))
                    .ForModel(new About {Heading = "En sida till"})
                    .ForUrl("/om-oss/en-sida-till")
                    .StoreAsync(new Page {Name = "En sida till", Metadata = new Metadata { MetaDescription = "Meta desc ...", MetaTitle = "Meta title SV ..." } });

                await session.SaveChangesAsync();
            }

        }

        class Configuration
        {
            public string Id { get; set; } 
        }
    }
}