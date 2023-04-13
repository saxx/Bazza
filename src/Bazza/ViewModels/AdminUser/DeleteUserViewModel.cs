using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminUser;

public class DeleteUserViewModelFactory
{
    private readonly Db _db;

    public DeleteUserViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<DeleteUserViewModel> Build(string username)
    {
        var user = await _db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username.ToLower() == username.ToLower()) ?? throw new EntityNotFoundException();
        var result = new DeleteUserViewModel
        {
            Username = user.Username,
            Sales = await _db.Sales.CountAsync(x => x.Username != null && x.Username.ToLower() == username.ToLower())
        };
        return result;
    }

    public async Task UpdateDatabase(DeleteUserViewModel viewModel)
    {
        if (!await _db.Users.AnyAsync(x => x.Username.ToLower() == viewModel.Username.ToLower())) throw new EntityNotFoundException();
        await _db.Users.Where(x => x.Username.ToLower() == viewModel.Username.ToLower()).ExecuteDeleteAsync();
        await _db.SaveChangesAsync();
    }
}

public class DeleteUserViewModel
{
    public string Username { get; init; } = "";
    public int Sales { get; set; }
}