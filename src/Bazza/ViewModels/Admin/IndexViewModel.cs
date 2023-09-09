using System.Collections.Generic;
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
            Articles = await _db.Articles.Select(x => new IndexViewModel.Article
            {
                IsBlocked = x.BlockedUtc.HasValue,
                IsInternal = x.PersonId > 1000,
                Price = x.Price,
                PersonId = x.PersonId,
                SaleId = x.SaleId
            }).ToListAsync()
        };
    }
}

public class IndexViewModel
{
    public IList<Article> Articles { get; set; } = new List<Article>();
    public IEnumerable<Article> ArticlesInternalOnly => Articles.Where(x => x.IsInternal);
    
    public class Article
    {
        public int PersonId { get; init; }
        public int? SaleId { get; set; }
        public bool IsInternal { get; init; }
        public bool IsBlocked { get; init; }
        public double Price { get; init; }

        public bool IsSold => SaleId.HasValue;
        public double ArticlesPercentage => IsInternal || !IsSold ? 0 : Price * Settings.PercentageProvision;
        public double ArticlesFee => IsInternal ? 0 : (Price < 25 ? Settings.CostsPerArticleBelow25 : Settings.CostsPerArticleAbove25);
    }
}