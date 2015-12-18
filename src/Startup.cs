using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Http;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Globalization;
using System.Globalization;

namespace aspnet5rc
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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            // Add localization to the request pipeline.
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                //DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("sv")
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("sv")
                }
            };
            requestLocalizationOptions.RequestCultureProviders.Insert(2, new RouteRequestCultureProvider()
            {
                Options = requestLocalizationOptions
            });
            app.UseRequestLocalization(requestLocalizationOptions, new RequestCulture("en"));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{culture=en}/{controller=Home}/{action=Index}/{id?}");                    
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
    }

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
                @"^/([a-z]{2})",
                RegexOptions.IgnoreCase);

            if (cultureValue.Success)
            {
                var culture = CultureInfoCache.GetCultureInfo(cultureValue.Groups[1].Value, Options.SupportedCultures);
                var uiCulture = CultureInfoCache.GetCultureInfo(cultureValue.Groups[1].Value, Options.SupportedCultures);

                if (culture == null || uiCulture == null)
                {
                    return Task.FromResult((ProviderCultureResult)null);
                }

                //if (culture.Name == Options.DefaultRequestCulture.Culture.TwoLetterISOLanguageName)
                //{
                //    // Redirect code goes here ..
                //    //httpContext.Response.Redirect("/en");
                //}

                var requestCulture = new ProviderCultureResult(culture.Name, uiCulture.Name);

                return Task.FromResult(requestCulture);
            }
            else
            {
                //var requestCulture = new ProviderCultureResult(Options.DefaultRequestCulture.Culture.TwoLetterISOLanguageName, Options.DefaultRequestCulture.UICulture.TwoLetterISOLanguageName);

                var requestCulture = new ProviderCultureResult("en", "en");

                return Task.FromResult(requestCulture);
            }

            //return Task.FromResult((ProviderCultureResult)null);
        }
    }
}
