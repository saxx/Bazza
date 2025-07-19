using System;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models;
using Bazza.Models.Database;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bazza.Services.BackgroundJobs;

public class CloseRegistrationBackgroundJob(IServiceProvider services)
{
    public async Task Run(PerformContext context)
    {
        using var scope = services.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<Db>();
        var settings = scope.ServiceProvider.GetRequiredService<Settings>();

        if (!settings.RegistrationHasStarted)
        {
            context.WriteLine("Registration has not started yet.");
            return;
        }

        if (settings.RegistrationHasEnded)
        {
            context.WriteLine("Registration has already ended.");
            return;
        }

        var users = await db.Articles.GroupBy(x => x.PersonId).Select(x => new
        {
            x.Key,
            Count = x.Count()
        }).ToListAsync();

        // if we have a list with only up to 10 articles, we count it as a full list. otherwise, we count the actual number.
        var articlesCount = 0;
        foreach (var u in users) articlesCount += u.Count <= 10 ? 50 : u.Count;

        if (articlesCount > settings.MaxNumberOfArticles)
        {
            context.WriteLine($"There are already {articlesCount} articles, but the limit is {settings.MaxNumberOfArticles}. Closing the registration now.");
            settings.RegistrationEndDate = DateTime.UtcNow.AddDays(-1).Date;
            await settings.UpdateDatabase();
        }
        else
        {
            context.WriteLine($"There are {articlesCount} articles, the limit is {settings.MaxNumberOfArticles}.");
        }
    }
}
