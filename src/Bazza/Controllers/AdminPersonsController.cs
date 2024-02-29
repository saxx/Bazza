using System;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Adliance.AspNetCore.Buddy.Template.Razor;
using Bazza.Models.Database;
using Bazza.Services;
using Bazza.ViewModels.AdminPersons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bazza.Controllers;

[Authorize(Roles = Roles.CanManagePersons)]
public class AdminPersonsController : Controller
{
    [HttpGet("/admin/persons")]
    public async Task<IActionResult> Persons([FromServices] PersonsViewModelFactory factory)
    {
        return View(await factory.Build());
    }

    [HttpGet("/admin/persons/statistics")]
    public async Task<IActionResult> PersonsStatistics([FromServices] PersonsStatisticsViewModelFactory factory)
    {
        return View(await factory.Build());
    }

    [HttpGet("/admin/persons/download-excel")]
    public async Task<IActionResult> Excel([FromServices] ExcelExportService excel)
    {
        var bytes = await excel.CreateExcelFile();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Basar_Neufelden.xlsx");
    }

    [HttpGet("/admin/person/{id}/pdf")]
    public async Task<IActionResult> Pdf([FromServices] ITemplater templater, [FromServices] IPdfer pdfer, [FromServices] EditPersonViewModelFactory factory, int id)
    {
        var viewModel = await factory.Build(id);
        var html = await templater.Render("Pdf", "Person", viewModel);
        var pdf = await pdfer.HtmlToPdf(html, new PdfOptions());
        return File(pdf, "application/pdf", $"Registrierung {viewModel.Id}.pdf");
    }

    [HttpGet("/admin/person/{id}/labels")]
    public async Task<IActionResult> Labels([FromServices] LabelsPdfService labelsPdfService, int id)
    {
        return File(await labelsPdfService.BuildPdf(id), "application/pdf", $"Labels ({id}).pdf");
    }

    [HttpGet("/admin/person")]
    public async Task<IActionResult> CreatePerson([FromServices] EditPersonViewModelFactory factory)
    {
        return View(nameof(EditPerson), await factory.Build((int?)null));
    }

    [HttpGet("/admin/person/{id}")]
    public async Task<IActionResult> EditPerson([FromServices] EditPersonViewModelFactory factory, int id)
    {
        return View(await factory.Build(id));
    }

    [HttpPost("/admin/person")]
    public async Task<IActionResult> CreatePerson([FromServices] EditPersonViewModelFactory factory, EditPersonViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(nameof(EditPerson), viewModel);
        await factory.UpdateDatabase(viewModel);
        return RedirectToAction(nameof(Persons));
    }

    [HttpPost("/admin/person/{id}")]
    public async Task<IActionResult> EditPerson([FromServices] EditPersonViewModelFactory factory, EditPersonViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        await factory.UpdateDatabase(viewModel);
        return RedirectToAction(nameof(Persons));
    }

    [HttpGet("/admin/person/{id}/delete/")]
    public async Task<IActionResult> DeletePerson([FromServices] DeletePersonViewModelFactory factory, int id)
    {
        return View(await factory.Build(id));
    }

    [HttpPost("/admin/person/{id}/delete/")]
    public async Task<IActionResult> DeletePerson([FromServices] DeletePersonViewModelFactory factory, DeletePersonViewModel viewModel)
    {
        await factory.UpdateDatabase(viewModel);
        return RedirectToAction(nameof(Persons));
    }

    [HttpPost("/admin/make-internal/{id}")]
    public async Task<IActionResult> MakeInternal([FromServices] Db db, [FromRoute] int id)
    {
        await ChangeId(db, id, 1001);
        return RedirectToAction(nameof(Persons));
    }

    [HttpPost("/admin/make-normal/{id}")]
    public async Task<IActionResult> MakeNormal([FromServices] Db db, [FromRoute] int id)
    {
        await ChangeId(db, id, 1);
        return RedirectToAction(nameof(Persons));
    }

    private static async Task ChangeId(Db db, int oldPersonId, int newPersonId)
    {
        var oldPerson = await db.Persons.SingleOrDefaultAsync(x => x.PersonId == oldPersonId);
        if (oldPerson == null) return;

        // find next free ID
        var existingPersonIds = await db.Persons.Select(x => x.PersonId).ToListAsync();
        while (existingPersonIds.Contains(newPersonId)) newPersonId++;

        // create new Person (because we can't change primary key)
        await db.Persons.AddAsync(new Person
        {
            Address = oldPerson.Address,
            Email = oldPerson.Email,
            Name = oldPerson.Name,
            Phone = oldPerson.Phone,
            AccessToken = oldPerson.AccessToken,
            CreatedUtc = oldPerson.CreatedUtc,
            PersonId = newPersonId,
            UpdatedUtc = DateTime.UtcNow
        });

        // create new articles because we can' change primary key
        var oldArticles = await db.Articles.Where(x => x.PersonId == oldPersonId).ToListAsync();
        foreach (var oldArticle in oldArticles)
        {
            await db.Articles.AddAsync(new Article
            {
                Name = oldArticle.Name,
                Price = oldArticle.Price,
                Size = oldArticle.Size,
                ArticleId = oldArticle.ArticleId,
                PersonId = newPersonId
            });
        }

        db.Articles.RemoveRange(oldArticles);
        db.Persons.Remove(oldPerson);
        await db.SaveChangesAsync();
    }
}
