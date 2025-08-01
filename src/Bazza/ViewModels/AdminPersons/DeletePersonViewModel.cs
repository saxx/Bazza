using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminPersons;

public class DeletePersonViewModelFactory(Db db)
{
    public async Task<DeletePersonViewModel> Build(int id)
    {
        var person = await db.Persons.AsNoTracking().SingleOrDefaultAsync(x => x.PersonId == id) ?? throw new EntityNotFoundException();
        var result = new DeletePersonViewModel
        {
            Id = person.PersonId,
            Name = person.Name,
            Articles = await db.Articles.CountAsync(x => x.PersonId == person.PersonId)
        };
        return result;
    }

    public async Task UpdateDatabase(DeletePersonViewModel viewModel)
    {
        await db.Articles.Where(x => x.PersonId == viewModel.Id).ExecuteDeleteAsync();
        await db.Persons.Where(x => x.PersonId == viewModel.Id).ExecuteDeleteAsync();
    }
}

public class DeletePersonViewModel
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public int Articles { get; set; }
}