using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
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
                return ActivatorUtilities.GetServiceOrCreateInstance<IConfiguration>(_serviceProvider);
            }
        }

        public static IDocumentStore DocumentStore => DocStore.Value;

        private static readonly Lazy<IDocumentStore> DocStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Urls = new string[] { Configuration["Data:DefaultConnection:ConnectionString"] },
                Database = Configuration["Data:DefaultDatabase"]
            };

            // try
            // {
            //   var dbRecord = new DatabaseRecord(store.Database);
            //   var createDbOp = new CreateDatabaseOperation(dbRecord);
            //   store.Maintenance.Server.Send(createDbOp);
            // }
            // catch (Exception e)
            // {
            //     // database already exists
            // }

            //store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(Configuration["Data:DefaultDatabase"]); // TODO Is this possible?
            //store.Conventions.RegisterIdLoadConvention<Site>((dbname, site) => "sites/" + site.Culture.Name);
            store.Conventions.RegisterAsyncIdConvention<Site>((dbname, site) => Task.FromResult($"sites/{site.Culture}"));

            //store.Conventions.RegisterIdConvention<Page>((databaseName, commands, page) => "pages/" + page.Id);
            //store.Conventions.RegisterAsyncIdConvention<ApplicationBuilderExtensions.MyClass>((dbname, commands, page) => new CompletedTask<string>("myclasses/" + page.PageLink));
            store.Initialize();
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
                //options.Filters.Add(typeof(PublishedFilterAttribute), 1);
                options.Filters.Add(typeof (AuthorizeFilterAttribute), 2);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddSingleton(DocumentStore);
            services.AddTransient<IRouteResolverTrie>(provider => new RouteResolverTrie(provider.GetService<IDocumentStore>()));

            services.AddTransient<IBricsContextAccessor>(provider => new BricsContextAccessor(provider.GetService<IHttpContextAccessor>(), provider.GetService<IDocumentStore>()));
        }
    }
}
