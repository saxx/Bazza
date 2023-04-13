using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminSales;

public class DeleteSaleViewModelFactory
{
    private readonly Db _db;

    public DeleteSaleViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<DeleteSaleViewModel> Build(int id)
    {
        var sale = await _db.Sales.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException();
        return new DeleteSaleViewModel
        {
            Id = sale.Id,
            ArticlesCount = await _db.Articles.CountAsync(x => x.SaleId == sale.Id)
        };
    }

    public async Task UpdateDatabase(DeleteSaleViewModel viewModel)
    {
        await _db.Articles.Where(x => x.SaleId == viewModel.Id).ExecuteUpdateAsync(c =>
            c.SetProperty(p => p.SaleId, _ => null)
                .SetProperty(p => p.SaleUtc, _ => null)
                .SetProperty(p => p.SaleUsername, _ => null)
        );
        await _db.Sales.Where(x => x.Id == viewModel.Id).ExecuteDeleteAsync();
    }
}

public class DeleteSaleViewModel
{
    public int Id { get; set; }
    public int ArticlesCount { get; set; }
}