using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminPersons;

public class PersonsViewModelFactory(Db db)
{
    public async Task<PersonsViewModel> Build()
    {
        return new PersonsViewModel
        {
            Persons = await db.Persons.OrderBy(x => x.PersonId).Select(x => new PersonsViewModel.Person
            {
                Address = x.Address,
                Email = x.Email,
                Id = x.PersonId,
                Name = x.Name,
                ArticlesCount = db.Articles.Count(y => y.PersonId == x.PersonId),
                ArticlesPrice = db.Articles.Where(y => y.PersonId == x.PersonId).Sum(y => y.Price),
                ArticlesSoldCount = db.Articles.Count(y => y.PersonId == x.PersonId && y.SaleId.HasValue),
                ArticlesSoldPrice = db.Articles.Where(y => y.PersonId == x.PersonId && y.SaleId.HasValue).Sum(y => y.Price)
            }).ToListAsync()
        };
    }
}

public class PersonsViewModel
{
    public IList<Person> Persons { get; init; } = new List<Person>();

    public record Person
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string? Address { get; init; }
        public int ArticlesCount { get; init; }
        public double ArticlesPrice { get; set; }
        
        public int ArticlesSoldCount { get; init; }
        public double ArticlesSoldPrice { get; set; }
    }
}