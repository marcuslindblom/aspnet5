using System;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Abstractions.Util;
using Raven.Client;
using Raven.Client.Document;
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
                    .AddJsonFile("config.json")
                    .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

                builder.AddEnvironmentVariables();
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
            store.Conventions.RegisterIdConvention<Site>((dbname, commands, site) => "sites/" + site.Culture.Name);
            store.Conventions.RegisterAsyncIdConvention<Site>((dbname, commands, site) => new CompletedTask<string>("sites/" + site.Culture.Name));
            return store;
        });

        public static void AddBrickPile(this IServiceCollection services)
        {
            _serviceProvider = services.BuildServiceProvider();

            services.AddMvc();

            services.ConfigureRouting(options =>
            {
                options.AppendTrailingSlash = true;
                options.LowercaseUrls = true;
            });

            services.Configure<MvcOptions>(options =>
            {
                options.ModelBinders.Insert(0,new DefaultModelBinder(DocumentStore));
                options.Filters.Add(typeof (AuthorizeFilterAttribute), 1);
            });

            services.AddInstance(DocumentStore);
            services.AddTransient<IRouteResolverTrie>(provider => new RouteResolverTrie(provider.GetService<IDocumentStore>()));
        }
    }
}
