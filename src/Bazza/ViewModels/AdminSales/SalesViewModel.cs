using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminSales;

public class SalesViewModelFactory
{
    private readonly Db _db;

    public SalesViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<SalesViewModel> Build()
    {
        return new SalesViewModel
        {
            Sales = await _db.Sales
                .OrderByDescending(x => x.CreatedUtc)
                .Select(x => new SalesViewModel.Sale
                {
                    Username = x.Username,
                    ArticlesCount = x.Articles.Count,
                    ArticlesSum = x.Articles.Sum(y => y.Price),
                    CreatedUtc = x.CreatedUtc,
                    Id = x.Id
                }).ToListAsync()
        };
    }
}

public class SalesViewModel
{
    public IList<Sale> Sales { get; set; } = new List<Sale>();

    public record Sale
    {
        public int Id { get; set; }
        public int ArticlesCount { get; set; }
        public double ArticlesSum { get; set; }
        public string? Username { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}