using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bazza
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
                            builder.AddApplicationInsights(instrumentationKey);
                        }
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
