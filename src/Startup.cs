using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using src;
using src.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace src
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBrickPile();
            services.AddTransient<LayoutModel>();

            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePages();

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
                options.DefaultRequestCulture = new RequestCulture("en");
            }); 

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }    
    //public class Startup
    //{
    //    public Startup(IHostingEnvironment env)
    //    {
    //        // Set up configuration sources.
    //        var builder = new ConfigurationBuilder()
    //            .AddJsonFile("appsettings.json")
    //            .AddEnvironmentVariables();
    //        Configuration = builder.Build();
    //    }

    //    public IConfigurationRoot Configuration { get; set; }

    //    // This method gets called by the runtime. Use this method to add services to the container.
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        // Add framework services.
    //        services.AddBrickPile();

    //        services.AddTransient<LayoutModel>();

    //        //services.AddMvc();            
    //    }

    //    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    //    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    //    {
    //        loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    //        loggerFactory.AddDebug();

    //        if (env.IsDevelopment())
    //        {
    //            app.UseDeveloperExceptionPage();
    //        }
    //        else
    //        {
    //            app.UseExceptionHandler("/Home/Error");
    //        }

    //        //app.UseIISPlatformHandler();

    //        app.UseStatusCodePages();

    //        app.UseStaticFiles();

    //        // Add localization to the request pipeline.
    //        app.UseBrickPile(options =>
    //        {                
    //            options.SupportedCultures = new List<CultureInfo>
    //            {
    //                new CultureInfo("en"),
    //                new CultureInfo("sv")
    //            };
    //            options.SupportedUICultures = new List<CultureInfo>
    //            {
    //                new CultureInfo("en"),
    //                new CultureInfo("sv")
    //            };
    //            options.DefaultRequestCulture = new RequestCulture("en");
    //        });               
    //    }
    //}
}
