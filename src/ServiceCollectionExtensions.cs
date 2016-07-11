using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Abstractions.Extensions;
using Raven.Abstractions.Util;
using Raven.Client;
using Raven.Client.Document;
using src.Models;
using src.Mvc;
using src.Mvc.ModelBinding;
using src.Routing.Trie;

namespace src
{
    public static class ServiceCollectionExtensions
    {
        private static IServiceProvider _serviceProvider;
        public static IConfiguration Configuration
        {
            get
            {
                var env = ActivatorUtilities.GetServiceOrCreateInstance<IHostingEnvironment>(_serviceProvider);
                //var appEnv = ActivatorUtilities.GetServiceOrCreateInstance<IApplicationEnvironment>(_serviceProvider);

                // Setup configuration sources.                                  
                var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();                
                return builder.Build();
            }
        }

        public static IDocumentStore DocumentStore => DocStore.Value;

        private static readonly Lazy<IDocumentStore> DocStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Url = Configuration["Data:DefaultConnection:ConnectionString"],
                DefaultDatabase = Configuration["Data:DefaultDatabase"]
            };

            store.Initialize();
            store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(Configuration["Data:DefaultDatabase"]);
            store.Conventions.RegisterIdConvention<Site>((dbname, commands, site) => "sites/" + site.Culture);
            store.Conventions.RegisterAsyncIdConvention<Site>((dbname, commands, site) => new CompletedTask<string>("sites/" + site.Culture));
            //store.Conventions.RegisterIdConvention<Page>((databaseName, commands, page) => "pages/" + page.Id);
            //store.Conventions.RegisterAsyncIdConvention<ApplicationBuilderExtensions.MyClass>((dbname, commands, page) => new CompletedTask<string>("myclasses/" + page.PageLink));
            return store;
        });

        public static void AddBrickPile(this IServiceCollection services)
        {
            _serviceProvider = services.BuildServiceProvider();

            services.AddMvc().ConfigureApplicationPartManager(manager =>
            {
                var feature = new ControllerFeature();
                manager.PopulateFeature(feature);
                services.AddSingleton<IControllerMapper>(new ControllerMapper(feature));
            });

            services.AddRouting(options =>
            {
                options.AppendTrailingSlash = true;
                options.LowercaseUrls = true;
            });

            services.Configure<MvcOptions>(options =>
            {
                options.ModelBinderProviders.Insert(0, new DefaultModelBinderProvider(DocumentStore));
                options.Filters.Add(typeof(PublishedFilterAttribute), 1);
                options.Filters.Add(typeof(AuthorizeFilterAttribute), 2);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(DocumentStore);
            services.AddTransient<IRouteResolverTrie>(provider => new RouteResolverTrie(provider.GetService<IDocumentStore>()));
            services.AddTransient<IBricsContextAccessor>(provider => new BricsContextAccessor(provider.GetService<IHttpContextAccessor>(), provider.GetService<IDocumentStore>()));
        }
    }
}
