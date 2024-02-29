using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Adliance.Buddy.DateTime;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminSales;

public class SalesStatisticsViewModelFactory
{
    private readonly Db _db;

    public SalesStatisticsViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<SalesStatisticsViewModel> Build()
    {
        var result = new SalesStatisticsViewModel();

        var soldArticles = await _db.Articles
            .Where(x => x.SaleId.HasValue && x.SaleUtc.HasValue)
            .OrderBy(x => x.SaleUtc)
            .Select(x => new
            {
                x.SaleUsername,
                SaleUtc = x.SaleUtc!.Value,
                x.Price
            }).ToListAsync();

        var groupedByUser = soldArticles.GroupBy(x => x.SaleUsername).Select(g => new SalesStatisticsViewModel.User
        {
            Username = g.Key ?? "< Unbekannt >",
            ArticlesCount = g.Count(),
            ArticlesPrice = g.Sum(x => x.Price)
        }).ToList();
        result.Users = groupedByUser.OrderByDescending(x => x.ArticlesCount).ToList();

        var firstSale = soldArticles.Min(x => x.SaleUtc.UtcToCet());
        var lastSale = soldArticles.Max(x => x.SaleUtc.UtcToCet());

        while (firstSale <= lastSale)
        {
            result.Hours.Add(new SalesStatisticsViewModel.Hour
            {
                Label = GetHourString(firstSale)
            });
            firstSale = firstSale.AddHours(1);
        }

        foreach (var a in soldArticles)
        {
            var hour = result.Hours.FirstOrDefault(x => x.Label == GetHourString(a.SaleUtc));
            if (hour != null)
            {
                hour.ArticlesCount += 1;
                hour.ArticlesPrice += a.Price;
            }
        }

        return result;
    }

    private static string GetHourString(DateTime d)
    {
        return d.UtcToCet().ToString("ddd, H 'Uhr'", new CultureInfo("de-DE")) ;
    }
}

public class SalesStatisticsViewModel
{
    public IList<User> Users { get; set; } = new List<User>();
    public IList<Hour> Hours { get; set; } = new List<Hour>();

    public record User
    {
        public string Username { get; init; } = "";
        public int ArticlesCount { get; init; }
        public double ArticlesPrice { get; init; }
    }

    public record Hour
    {
        public string Label { get; set; } = "";
        public int ArticlesCount { get; set; }
        public double ArticlesPrice { get; set; }
    }
}
