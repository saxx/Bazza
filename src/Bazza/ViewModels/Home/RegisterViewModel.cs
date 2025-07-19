using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy;
using Adliance.AspNetCore.Buddy.Abstractions;
using Adliance.Buddy.Crypto;
using Bazza.Models.Database;
using Bazza.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bazza.ViewModels.Home;

public class RegisterViewModelFactory(ILogger<RegisterViewModelFactory> logger, Db db, IEmailer emailer, LinkGenerator link, IHttpContextAccessor context)
{
    public async Task<RegisterViewModel> Fill(string? accessToken = null)
    {
        var result = new RegisterViewModel();

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            var person = await db.Persons.FirstOrDefaultAsync(x => x.AccessToken == accessToken);
            if (person != null)
            {
                result.Address = person.Address;
                result.Email = person.Email;
                result.Name = person.Name;
                result.Phone = person.Phone;
                result.AccessToken = person.AccessToken;

                var articles = await db.Articles.Where(x => x.PersonId == person.PersonId).OrderBy(x => x.ArticleId).ToListAsync();
                result.Articles = articles.Select(x => new RegisterViewModel.Article
                {
                    Name = x.Name,
                    Price = x.Price,
                    Size = x.Size,
                    Sold = x.SaleUtc.HasValue
                }).ToList();
            }
        }

        return result;
    }

    public async Task SaveToDatabase(RegisterViewModel viewModel)
    {
        var sendMail = false;

        Person? person = null;
        if (!string.IsNullOrWhiteSpace(viewModel.AccessToken))
        {
            person = await db.Persons.FirstOrDefaultAsync(x => x.AccessToken == viewModel.AccessToken);
        }

        if (person != null && await db.Articles.AnyAsync(x => x.PersonId == person.PersonId && x.SaleUtc.HasValue))
        {
            throw new Exception("Unable to update registration after first sale. This exception should not be possible as the UI hides the 'Submit' button if something has been sold.");
        }

        if (person == null)
        {
            person = await CreateNewPerson(viewModel);
            db.Persons.Add(person);
            sendMail = true;
        }
        else
        {
            person.UpdatedUtc = DateTime.UtcNow;
        }

        person.Address = viewModel.Address;
        person.Email = viewModel.Email;
        person.Phone = viewModel.Phone;
        person.Name = viewModel.Name;
        person.AccessToken = string.IsNullOrWhiteSpace(person.AccessToken) ? Crypto.RandomString(20) : person.AccessToken;
        db.Articles.RemoveRange(db.Articles.Where(x => x.PersonId == person.PersonId));
        await db.SaveChangesAsync();

        var articleId = 1;
        foreach (var a in viewModel.Articles)
        {
            db.Articles.Add(new Article
            {
                Name = a.Name,
                Price = a.Price ?? 0,
                Size = a.Size,
                ArticleId = articleId++,
                PersonId = person.PersonId
            });
        }

        await db.SaveChangesAsync();

        viewModel.AccessToken = person.AccessToken;

        if (sendMail)
        {
            viewModel.DisplayInitialSuccess = true;
            await SendEmail(viewModel);
        }
        else
        {
            viewModel.DisplaySubsequentSuccess = true;
        }
    }

    private async Task SendEmail(RegisterViewModel viewModel)
    {
        try
        {
            var subject = "Deine Registrierung für den Basar Neufelden";
            var htmlBody = "<div style=\"font-family:sans-serif;\">" +
                           $"Hallo {viewModel.Name}!<br /><br />Danke für deine Registrierung für den Basar Neufelden. Wir freuen uns, dass du mit an Bord bist.<br /><br />" +
                           "Wenn du deine Registrierung oder deine Artikel im Nachhinein anpassen möchtest, kannst du das über die folgende Adresse tun:<br /><br />" +
                           $"{link.GetUriByAction(context.HttpContext!, "Register", "Home", null, "https") + "/" + viewModel.AccessToken}<br /><br />" +
                           $"Für Fragen haben wir eine detaillierte Anleitung zusammengestellt: {link.GetUriByAction(context.HttpContext!, "Manual", "Home", null, "https")}.<br /><br />" +
                           "Danke & liebe Grüße,<br />das Team vom Basar Neufelden" +
                           "</div>";

            await emailer.Send(viewModel.Email ?? "", subject, htmlBody, "");
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Unable to send email: {ex.Message}");
        }
    }

    private async Task<Person> CreateNewPerson(RegisterViewModel viewModel)
    {
        var existingPersonIds = await db.Persons.Select(x => x.PersonId).ToListAsync();
        var personId = (viewModel.Address ?? "").Contains("Mütterrunde", StringComparison.InvariantCultureIgnoreCase) ? 1001 : 1;
        while (existingPersonIds.Contains(personId))
        {
            personId++;
        }

        return new Person
        {
            PersonId = personId,
            CreatedUtc = DateTime.UtcNow
        };
    }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Bitte gib deinen Namen an.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Bitte gib deine Adresse an.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Bitte gib deine E-Mail-Adresse an."), EmailAddress(ErrorMessage = "Bitte gib deine korrekte E-Mail-Adresse an.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Bitte gib deine Telefonnummer an.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Bitte stimm unserer Datenschutzerklärung zu."), RegularExpression("true", ErrorMessage = "Bitte stimme unserer Datenschutzerklärung zu.")]
    public string? Privacy { get; set; }

    public IList<Article> Articles { get; set; } = new List<Article>();

    public string? AccessToken { get; set; }

    [BindNever] public bool DisplayInitialSuccess { get; set; }
    [BindNever] public bool DisplaySubsequentSuccess { get; set; }

    public class Article
    {
        [Required(ErrorMessage = "Bitte gib eine aussagekräftige Artikelbeschreibung an.")]
        public string? Name { get; init; }

        public string? Size { get; init; }

        [Required(ErrorMessage = "Bitte gib den Preis an."), DivisableBy50, RegularExpression("[\\d,]*", ErrorMessage = "Bitte gib den Preis an.")]
        [Range(0.1, 99999, ErrorMessage = "Der Preis muss größer als 0,- sein.")]
        public double? Price { get; init; }

        public bool Sold { get; set; }
    }
}
