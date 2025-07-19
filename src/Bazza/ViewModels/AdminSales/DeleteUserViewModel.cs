using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminSales;

public class DeleteSaleViewModelFactory(Db db)
{
    public async Task<DeleteSaleViewModel> Build(int id)
    {
        var sale = await db.Sales.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException();
        return new DeleteSaleViewModel
        {
            Id = sale.Id,
            ArticlesCount = await db.Articles.CountAsync(x => x.SaleId == sale.Id)
        };
    }

    public async Task UpdateDatabase(DeleteSaleViewModel viewModel)
    {
        await db.Articles.Where(x => x.SaleId == viewModel.Id).ExecuteUpdateAsync(c =>
            c.SetProperty(p => p.SaleId, _ => null)
                .SetProperty(p => p.SaleUtc, _ => null)
                .SetProperty(p => p.SaleUsername, _ => null)
        );
        await db.Sales.Where(x => x.Id == viewModel.Id).ExecuteDeleteAsync();
    }
}

public class DeleteSaleViewModel
{
    public int Id { get; set; }
    public int ArticlesCount { get; set; }
}