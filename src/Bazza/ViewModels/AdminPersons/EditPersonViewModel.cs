using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminPersons;

public class EditPersonViewModelFactory
{
    private readonly Db _db;

    public EditPersonViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<EditPersonViewModel> Build(string accessToken)
    {
        var person = await _db.Persons.AsNoTracking().SingleOrDefaultAsync(x => x.AccessToken == accessToken) ?? throw new EntityNotFoundException();
        return await Build(person.PersonId);
    }

    public async Task<EditPersonViewModel> Build(int? id)
    {
        EditPersonViewModel result;
        if (!id.HasValue)
        {
            result = new EditPersonViewModel();
        }
        else
        {
            var person = await _db.Persons.AsNoTracking().SingleOrDefaultAsync(x => x.PersonId == id) ?? throw new EntityNotFoundException();
            result = new EditPersonViewModel
            {
                Id = person.PersonId,
                Address = person.Address ?? "",
                IsInternal = person.PersonId > 1000,
                Email = person.Email ?? "",
                Name = person.Name ?? "",
                Phone = person.Phone,
                Articles = await _db.Articles
                    .Where(x => x.PersonId == person.PersonId)
                    .OrderBy(x => x.ArticleId)
                    .Select(x => new EditPersonViewModel.Article
                    {
                        Name = x.Name,
                        Price = x.Price,
                        Size = x.Size,
                        SaleUtc = x.SaleUtc,
                        SaleId = x.SaleId,
                        SaleUsername = x.SaleUsername
                    }).ToListAsync()
            };
        }

        while (result.Articles.Count < 50) result.Articles.Add(new EditPersonViewModel.Article());
        return result;
    }

    public async Task UpdateDatabase(EditPersonViewModel viewModel)
    {
        Person person;
        if (viewModel.Id.HasValue)
        {
            person = await _db.Persons.SingleOrDefaultAsync(x => x.PersonId == viewModel.Id.Value) ?? throw new EntityNotFoundException();
        }
        else
        {
            person = await CreateNewPerson(viewModel.Address);
            await _db.Persons.AddAsync(person);
        }

        person.Name = viewModel.Name;
        person.Email = viewModel.Email;
        person.Address = viewModel.Address;
        person.Phone = viewModel.Phone;

        await _db.SaveChangesAsync();

        for (var i = 0; i < viewModel.Articles.Count; i++)
        {
            var existingArticle = await _db.Articles.SingleOrDefaultAsync(x => x.PersonId == person.PersonId && x.ArticleId == i + 1);
            if (existingArticle == null && !string.IsNullOrWhiteSpace(viewModel.Articles[i].Name))
            {
                await _db.Articles.AddAsync(new Article
                {
                    Name = viewModel.Articles[i].Name,
                    Size = viewModel.Articles[i].Size,
                    Price = viewModel.Articles[i].Price ?? 0,
                    ArticleId = i + 1,
                    PersonId = person.PersonId
                });
            }
            else if (existingArticle != null && !string.IsNullOrWhiteSpace(viewModel.Articles[i].Name))
            {
                existingArticle.Name = viewModel.Articles[i].Name;
                existingArticle.Size = viewModel.Articles[i].Size;
                existingArticle.Price = viewModel.Articles[i].Price ?? 0;
            }
            else if (existingArticle != null && string.IsNullOrWhiteSpace(viewModel.Articles[i].Name) && !existingArticle.SaleId.HasValue)
            {
                _db.Articles.Remove(existingArticle);
            }
        }

        await _db.SaveChangesAsync();
    }

    private async Task<Person> CreateNewPerson(string address)
    {
        var existingPersonIds = await _db.Persons.Select(x => x.PersonId).ToListAsync();
        var personId = address.Contains("Mütterrunde", StringComparison.InvariantCultureIgnoreCase) ? 1001 : 1;
        while (existingPersonIds.Contains(personId)) personId++;
        return new Person
        {
            PersonId = personId,
            CreatedUtc = DateTime.UtcNow
        };
    }
}

public class EditPersonViewModel
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "Bitte Name angeben.")] public string Name { get; set; } = "";
    [Required(ErrorMessage = "Bitte E-Mail-Adresse angeben.")] public string Email { get; set; } = "";
    [Required(ErrorMessage = "Bitte Adresse angeben.")] public string Address { get; set; } = "";
    public string? Phone { get; set; }
    public bool IsInternal { get; set; }

    public IList<Article> Articles { get; set; } = new List<Article>();

    public int ArticlesSold => Articles.Count(x => x.IsSold);
    public double ArticlesSoldPrice => Articles.Where(x => x.IsSold).Sum(x => x.Price ?? 0);
    public double ArticlesPercentage => IsInternal ? 0 : ArticlesSoldPrice * 0.2;
    public double ArticlesFee => IsInternal ? 0 : Articles.Count(x => x.Price < 25) * 0.1 + Articles.Count(x => x.Price >= 25) * 0.1;
    public double Payout => ArticlesSoldPrice - ArticlesPercentage - ArticlesFee > 0 ? ArticlesSoldPrice - ArticlesPercentage - ArticlesFee : 0;

    public record Article
    {
        public string? Name { get; init; }
        public string? Size { get; init; }
        public double? Price { get; init; }
        public bool IsSold => SaleId.HasValue;
        public int? SaleId { get; init; }
        public DateTime? SaleUtc { get; init; }
        public string? SaleUsername { get; init; }
    }
}