using System;
using Microsoft.Extensions.DependencyInjection;

namespace Bazza.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationInsightsIfConfigured(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddApplicationInsightsTelemetry();
        }
    }
}
