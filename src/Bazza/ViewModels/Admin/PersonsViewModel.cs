using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.Admin;

public class PersonsViewModelFactory
{
    private readonly Db _db;

    public PersonsViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<PersonsViewModel> Build()
    {
        var result = new PersonsViewModel();
        result.Persons = await _db.Persons.OrderBy(x => x.PersonId).Select(x => new PersonsViewModel.Person
        {
            Address = x.Address,
            Email = x.Email,
            Id = x.PersonId,
            Name = x.Name,
            AccessToken = x.AccessToken,
            ArticlesCount = _db.Articles.Count(y => y.PersonId == x.PersonId),
            IsInternal = x.PersonId > 1000
        }).ToListAsync();
        return result;
    }
}

public class PersonsViewModel
{
    public IList<Person> Persons { get; set; } = new List<Person>();

    public record Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? AccessToken { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int ArticlesCount { get; set; }
        public bool IsInternal { get; set; }
    }
}