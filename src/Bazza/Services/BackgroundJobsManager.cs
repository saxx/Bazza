using System;
using Bazza.Services.BackgroundJobs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bazza.Services;

public class BackgroundJobsManager(IServiceProvider services)
{
    private void Start()
    {
        RecurringJob.AddOrUpdate(nameof(CloseRegistrationBackgroundJob), () => new CloseRegistrationBackgroundJob(services).Run(null!), Cron.Hourly);
    }

    public static void ConfigureServices(IServiceCollection services, string dbConnectionString)
    {
        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(dbConnectionString).WithJobExpirationTimeout(TimeSpan.FromDays(30));
            config.UseConsole();
        });
        services.AddHangfireServer(x => { x.WorkerCount = 1; });
    }

    public void Start(IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/admin/hangfire", new DashboardOptions
        {
            Authorization = new[]
            {
                new HangfireAuthorizationFilter()
            }
        });

        Start();
    }
}

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var user = context.GetHttpContext()?.User;
        if (user == null) return false;
        if (!user.IsInRole(Roles.CanManageAdmin)) return false;
        return user.Identity?.IsAuthenticated ?? false;
    }
}
