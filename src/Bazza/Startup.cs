using System.Globalization;
using System.Xml;
using Adliance.AspNetCore.Buddy.Email;
using Adliance.AspNetCore.Buddy.Extensions;
using Bazza.Models.Database;
using Bazza.Services;
using Bazza.ViewModels.Home;
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
            services.AddControllersWithViews(options => { options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((a,b) => "Ungültige Angabe."); });
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

            services.AddHttpContextAccessor();
            services.AddTransient<IEmailer, SendgridEmailer>();
            services.AddTransient<ISendgridConfiguration>(_ => new SendgridConfiguration(_configuration.GetValue<string>("SendgridSecret")));
            services.AddTransient<IEmailConfiguration>(_ => new EmailConfiguration());
            services.AddTransient<IndexViewModelFactory>();
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

        public class SendgridConfiguration : ISendgridConfiguration
        {
            public SendgridConfiguration(string secret)
            {
                SendgridSecret = secret;
            }

            public string SendgridSecret { get; }
            public string SendgridLabel => "Bazza";
        }

        public class EmailConfiguration : IEmailConfiguration
        {
            public string EmailSenderName => "Basar Neufelden";
            public string EmailSenderAddress => "basar.neufelden@sendgrid.me";
        }
    }
}