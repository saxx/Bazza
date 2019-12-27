using System.Globalization;
using Bazza.Models.Database;
using Bazza.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bazza
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddDbContext<Db>(options => options.UseSqlServer(_configuration.GetValue<string>("DbConnectionString")));
            services.AddControllersWithViews();
            services.AddWebOptimizer(pipeline =>
            {
                pipeline.AddScssBundle(
                    "/css/bundle.css",
                    "/css/site.scss");
                pipeline.AddJavaScriptBundle(
                    "/js/bundle.js",
                    "/lib/jquery.js",
                    "/js/site.js");
            });
            services.AddHealthChecks().AddDbContextCheck<Db>();

            services.AddTransient<ExcelExportService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStatusCodePages();
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("de-DE", "de-DE");
                options.SupportedCultures = new[] {new CultureInfo("de-DE")};
                options.SupportedUICultures = new[] {new CultureInfo("de-DE")};
            });
            app.UseRouting();
            app.UseWebOptimizer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}