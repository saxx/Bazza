using System.Linq;
using System.Threading.Tasks;
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
    
    public int ArticlesBlocked { get; init; }
    public double ArticlesPriceBlocked { get; init; }
}