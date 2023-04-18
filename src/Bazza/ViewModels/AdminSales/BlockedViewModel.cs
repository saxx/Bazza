using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminSales;

public class BlockedViewModelFactory
{
    private readonly Db _db;

    public BlockedViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<BlockedViewModel> Build()
    {
        var result = new BlockedViewModel
        {
            Articles = await _db.Articles
                .Where(x => x.BlockedUtc.HasValue)
                .OrderBy(x => x.BlockedUtc)
                .Select(x => new BlockedViewModel.Article
                {
                    Name = x.Name,
                    Price = x.Price,
                    ArticleId = x.ArticleId,
                    PersonId = x.PersonId,
                    Size = x.Size,
                    BlockedUtc = x.BlockedUtc,
                    BlockedUsername = x.BlockedUsername
                }).ToListAsync()
        };
        return result;
    }
}

public class BlockedViewModel
{
    public bool DisplayAlreadySoldError { get; set; }
    public bool DisplayInvalidError { get; set; }

    public IList<Article> Articles { get; set; } = new List<Article>();

    public record Article
    {
        public int PersonId { get; init; }
        public int ArticleId { get; init; }
        public string? Name { get; init; }
        public string? Size { get; init; }
        public double Price { get; init; }
        public DateTime? BlockedUtc { get; init; }
        public string? BlockedUsername { get; init; }
    }
}