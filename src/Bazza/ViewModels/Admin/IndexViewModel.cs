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
            Users = await _db.Users.CountAsync()
        };
    }
}

public class IndexViewModel
{
    public int Persons { get; set; }
    public int Articles { get; set; }
    public int Users { get; set; }
}