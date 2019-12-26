using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Bazza
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddControllersWithViews();
            services.AddWebOptimizer(pipeline =>
            {
                pipeline.AddScssBundle(
                    "/css/bundle.css",
                    "/css/site.scss");
                pipeline.AddJavaScriptBundle(
                    "/js/bundle.js",
                    "/js/site.js");
            });
            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseExceptionHandler("/error/500");
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