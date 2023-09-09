using System.Linq;
using System.Threading.Tasks;
using Bazza.Models;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.Admin;

public class IndexViewModelFactory
{
    private readonly Db _db;

    public IndexViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<IndexViewModel> Build()
    {
        return new IndexViewModel
        {
            Articles = await _db.Articles.CountAsync(),
            Persons = await _db.Persons.CountAsync(),
            ArticlesPrice = await _db.Articles.SumAsync(x => x.Price),
            ArticlesSold = await _db.Articles.CountAsync(x => x.SaleId.HasValue),
            ArticlesPriceSold = await _db.Articles.Where(x => x.SaleId.HasValue).SumAsync(x => x.Price),
            ArticlesBlocked = await _db.Articles.CountAsync(x => x.BlockedUtc.HasValue),
            ArticlesPriceBlocked = await _db.Articles.Where(x => x.BlockedUtc.HasValue).SumAsync(x => x.Price),
            ProvisionPerArticle =
                await _db.Articles.CountAsync(x => x.Price > 25 && x.SaleId.HasValue) * Settings.CostsPerArticleAbove25 +
                await _db.Articles.CountAsync(x => x.Price <= 25 && x.SaleId.HasValue) * Settings.CostsPerArticleBelow25,
            ProvisionPercent = await _db.Articles.Where(x => x.SaleId.HasValue).SumAsync(x => x.Price) * Settings.PercentageProvision,
            Sales = await _db.Sales.CountAsync()
        };
    }
}

public class IndexViewModel
{
    public int Persons { get; init; }
    public int Articles { get; init; }
    public double ArticlesPrice { get; init; }
    public int Sales { get; init; }
    public int ArticlesSold { get; init; }
    public double ArticlesPriceSold { get; init; }

    public double ProvisionPercent { get; set; }
    public double ProvisionPerArticle { get; set; }

    public int ArticlesBlocked { get; init; }
    public double ArticlesPriceBlocked { get; init; }
}