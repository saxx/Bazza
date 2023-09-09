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
        var result = new IndexViewModel
        {
            Articles = await _db.Articles.Select(x => new IndexViewModel.Article
            {
                IsBlocked = x.BlockedUtc.HasValue,
                IsInternal = x.PersonId > 1000,
                Price = x.Price,
                PersonId = x.PersonId,
                IsSold = x.SaleId.HasValue
            }).ToListAsync(),
            SalesCount = await _db.Sales.CountAsync(),
            EmptySalesCount = await _db.Sales.Where(x => !x.Articles.Any()).CountAsync()
        };

        foreach (var article in result.Articles)
        {
            foreach (var a in result.Articles.Where(x => x.PersonId == article.PersonId))
            {
                if (article.IsSold)
                {
                    a.RegistrationHasAnySold = true;
                    a.RegistrationTotalSold += article.Price;
                }
                a.RegistrationTotalProvision += article.Provision;
            }
        }

        return result;
    }
}

public class IndexViewModel
{
    public IList<Article> Articles { get; set; } = new List<Article>();
    public IEnumerable<Article> ArticlesInternalOnly => Articles.Where(x => x.IsInternal);
    public int SalesCount { get; set; }
    public int EmptySalesCount { get; set; }

    public class Article
    {
        public int PersonId { get; init; }
        public bool IsInternal { get; init; }
        public bool IsBlocked { get; init; }
        public double Price { get; init; }
        public bool IsSold { get; init; }

        public bool RegistrationHasAnySold { get; set; }
        public double RegistrationTotalSold { get; set; }
        public double RegistrationTotalProvision { get; set; }

        public bool IsExemptFromFees => !RegistrationHasAnySold || (RegistrationTotalSold < RegistrationTotalProvision);

        public double Percentage => IsInternal || !IsSold ? 0 : Price * Settings.PercentageProvision;

        public double Fee => IsInternal ? 0 : (Price < 25 ? Settings.CostsPerArticleBelow25 : Settings.CostsPerArticleAbove25);

        public double Provision => Percentage + Fee;
    }
}