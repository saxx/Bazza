using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminSales;

public class SaleViewModelFactory(Db db)
{
    public async Task<SaleViewModel> Build(int id)
    {
        var sale = await db.Sales.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException();
        var result = new SaleViewModel
        {
            Id = sale.Id,
            CreatedUtc = sale.CreatedUtc,
            Username = sale.Username,
            Articles = await db.Articles
                .Where(x => x.SaleId == sale.Id)
                .OrderByDescending(x => x.SaleUtc)
                .Select(x => new SaleViewModel.Article
                {
                    Name = x.Name,
                    Price = x.Price,
                    ArticleId = x.ArticleId,
                    PersonId = x.PersonId,
                    Size = x.Size,
                    SaleUtc = x.SaleUtc
                }).ToListAsync()
        };
        return result;
    }
}

public class SaleViewModel
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedUtc { get; set; }
    public double ArticlesPriceSum => Articles.Sum(x => x.Price);
    public int ArticlesCount => Articles.Count;

    public bool DisplayAlreadySoldError { get; set; }
    public bool DisplayLockedError { get; set; }
    public bool DisplayInvalidError { get; set; }
    
    public IList<Article> Articles { get; set; } = new List<Article>();

    public record Article
    {
        public int PersonId { get; set; }
        public int ArticleId { get; set; }
        public string? Name { get; set; }
        public string? Size { get; set; }
        public double Price { get; set; }
        public DateTime? SaleUtc { get; set; }
    }
}