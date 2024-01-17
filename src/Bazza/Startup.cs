using System;
using System.Globalization;
using Adliance.AspNetCore.Buddy.Abstractions.Extensions;
using Adliance.AspNetCore.Buddy.Email.Mailjet.Extensions;
using Adliance.AspNetCore.Buddy.Pdf.V2.Extensions;
using Adliance.AspNetCore.Buddy.Template.Razor.Extensions;
using Bazza.Models;
using Bazza.Models.Database;
using Bazza.Services;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bazza;

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

        services.AddScoped<Settings>();
        services.AddTransient<ExcelExportService>();
        services.AddTransient<LabelsPdfService>();
        services.AddTransient<ViewModels.Admin.IndexViewModelFactory>();
        services.AddTransient<ViewModels.Admin.SettingsViewModelFactory>();
        services.AddTransient<ViewModels.AdminPersons.DeletePersonViewModelFactory>();
        services.AddTransient<ViewModels.AdminPersons.PersonsStatisticsViewModelFactory>();
        services.AddTransient<ViewModels.AdminPersons.PersonsViewModelFactory>();
        services.AddTransient<ViewModels.AdminPersons.EditPersonViewModelFactory>();
        services.AddTransient<ViewModels.AdminSales.BlockedViewModelFactory>();
        services.AddTransient<ViewModels.AdminSales.DeleteSaleViewModelFactory>();
        services.AddTransient<ViewModels.AdminSales.SalesStatisticsViewModelFactory>();
        services.AddTransient<ViewModels.AdminSales.SalesViewModelFactory>();
        services.AddTransient<ViewModels.AdminSales.SaleViewModelFactory>();
        services.AddTransient<ViewModels.AdminUsers.DeleteUserViewModelFactory>();
        services.AddTransient<ViewModels.AdminUsers.EditUserViewModelFactory>();
        services.AddTransient<ViewModels.AdminUsers.UsersViewModelFactory>();
        services.AddTransient<ViewModels.Home.RegisterViewModelFactory>();
        services.AddTransient<ViewModels.User.LoginViewModelFactory>();
        services.AddTransient<ViewModels.User.ResetPasswordViewModelFactory>();

        services.AddDbContext<Db>(options => options.UseSqlServer(_configuration.GetValue<string>("DbConnectionString") ?? ""));
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AuthorizeFilter());
            options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((_, _) => "Ungültige Angabe.");
        });
        
        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName).AddV8();
        services.AddWebOptimizer(pipeline =>
        {
            pipeline.AddScssBundle(
                "/css/bundle.css",
                "/css/site.scss");
            pipeline.AddJavaScriptBundle(
                "/js/bundle.js",
                "/lib/jquery.js",
                "/lib/echarts.js",
                "/js/site.js");
        });
        services.AddHealthChecks()
            .AddMailjetCheck()
            .AddDbContextCheck<Db>();
        services.AddAuthorization();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.Name = "auth";
                    options.ExpireTimeSpan = TimeSpan.FromHours(12);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/error/403";
                    options.LoginPath = "/user/login";
                }
            );

        services.AddBuddy()
            .AddMailjet(_configuration.GetSection("Email"), _configuration.GetSection("Mailjet"))
            .AddPdf(_configuration.GetSection("Pdf"))
            .AddRazorPdfV2Renderer();
        services.AddHttpContextAccessor();
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseHsts();
        app.UseStatusCodePagesWithRedirects("/error/{0}");
        app.UseExceptionHandler("/error/500");
        app.UseStaticFiles();
        app.UseRequestLocalization(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("de-DE", "de-DE");
            options.SupportedCultures = new[] { new CultureInfo("de-DE") };
            options.SupportedUICultures = new[] { new CultureInfo("de-DE") };
        });
        app.UseRouting();
        app.UseWebOptimizer();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapHealthChecks("/health");
        });
    }
}