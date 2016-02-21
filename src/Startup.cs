using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Localization;
using System.Globalization;
using src;
using src.Models;
using Microsoft.AspNet.FileProviders;

namespace src
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddBrickPile();

            services.AddTransient<LayoutModel>();

            //services.AddMvc();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseStatusCodePagesWithRedirects("~/{0}.html");
                //app.UseStatusCodePagesWithReExecute("/{0}.html");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            //app.UseStatusCodePages();

            app.UseStaticFiles();

            // Add localization to the request pipeline.
            app.UseBrickPile(options =>
            {                
                options.SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("sv")
                };
                options.SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("sv")
                };
                //options.DefaultRequestCulture = new RequestCulture("en");
            });
                
            

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default_localization",
            //        template: "{culture?}/{controller=Home}/{action=Index}/{id?}");
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
        //public static void Main(string[] args)
        //{
        //    var application = new WebApplicationBuilder()
        //        .UseConfiguration(WebApplicationConfiguration.GetDefault(args))
        //        .UseStartup<Startup>()
        //        .Build();

        //    application.Run();
        //}
    }
}
