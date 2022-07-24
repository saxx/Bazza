using System;
using System.Linq;
using Bazza.Models.Database;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bazza;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var context = services.GetRequiredService<Db>();
            try
            {
                var pendingMigrationsCount = context.Database.GetPendingMigrations().Count();
                if (context.Database.GetPendingMigrations().Any())
                {
                    logger.LogWarning($"Applying {pendingMigrationsCount} {"migration".ToQuantity(pendingMigrationsCount)} ...");
                    context.Database.SetCommandTimeout(60 * 10); // 10m timeout
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Unable to apply migrations.");
            }
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureLogging((context, builder) =>
                {
                    var instrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                    if (!string.IsNullOrWhiteSpace(instrumentationKey))
                    {
#pragma warning disable CS0618
                        builder.AddApplicationInsights(instrumentationKey);
#pragma warning restore CS0618
                    }
                });
                webBuilder.UseStartup<Startup>();
            });
}