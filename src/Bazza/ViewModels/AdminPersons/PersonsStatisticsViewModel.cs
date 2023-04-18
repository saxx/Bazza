using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminPersons;

public class PersonsStatisticsViewModelFactory
{
    private readonly Db _db;

    public PersonsStatisticsViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<PersonsStatisticsViewModel> Build()
    {
        var result = new PersonsStatisticsViewModel();
        
        var soldArticles = await _db.Articles
            .Where(x => x.SaleId.HasValue)
            .OrderBy(x => x.SaleUtc)
            .Select(x => new
            {
                x.PersonId,
                x.Price
            }).ToListAsync();

        var persons = await _db.Persons.ToDictionaryAsync(x => x.PersonId, x => x.Name);
        var groupedByPerson = soldArticles.GroupBy(x => x.PersonId).Select(g => new PersonsStatisticsViewModel.Person
        {
            Name = persons[g.Key] ?? "< Unbekannt >",
            ArticlesCount = g.Count(),
            ArticlesPrice = g.Sum(x => x.Price)
        }).ToList();
        result.Persons = groupedByPerson.OrderByDescending(x => x.ArticlesCount).ToList();
        return result;
    }
}

public class PersonsStatisticsViewModel
{
    public IList<Person> Persons { get; set; } = new List<Person>();

    public record Person
    {
        public string Name { get; init; } = "";
        public int ArticlesCount { get; init; }
        public double ArticlesPrice { get; init; }
    }
}