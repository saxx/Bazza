using System;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Bazza.Services;
using Bazza.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bazza.Controllers;

public class AdminController : Controller
{
    [HttpGet("/admin")]
    public async Task<IActionResult> Index([FromServices] IndexViewModelFactory factory)
    {
        return View(await factory.Build());
    }
    
    [HttpGet("/admin/persons")]
    public async Task<IActionResult> Persons([FromServices] PersonsViewModelFactory factory)
    {
        return View(await factory.Build());
    }

    [HttpGet("/admin/settings")]
    public async Task<IActionResult> Settings([FromServices] SettingsViewModelFactory factory)
    {
        return View(await factory.Build());
    }

    [HttpPost("/admin/settings")]
    public async Task<IActionResult> Settings([FromServices] SettingsViewModelFactory factory, SettingsViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        await factory.UpdateDatebase(viewModel);
        return View(viewModel);
    }

    [HttpGet("/admin/download-excel")]
    public async Task<IActionResult> Download([FromServices] ExcelExportService excel)
    {
        var bytes = await excel.CreateExcelFile();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Basar_Neufelden_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }

    [HttpPost("/admin/delete-person/{id}")]
    public async Task<IActionResult> DeletePerson([FromServices] Db db, [FromRoute] int id)
    {
        db.Articles.RemoveRange(db.Articles.Where(x => x.PersonId == id));
        db.Persons.RemoveRange(db.Persons.Where(x => x.PersonId == id));
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/make-internal/{id}")]
    public async Task<IActionResult> MakeInternal([FromServices] Db db, [FromRoute] int id)
    {
        await ChangeId(db, id, 1001);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/make-normal/{id}")]
    public async Task<IActionResult> MakeNormal([FromServices] Db db, [FromRoute] int id)
    {
        await ChangeId(db, id, 1);
        return RedirectToAction(nameof(Index));
    }

    private async Task ChangeId(Db db, int oldPersonId, int newPersonId)
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

    [HttpGet("/admin/clear-data")]
    public IActionResult ClearData([FromServices] Db db)
    {
        return View();
    }

    [HttpPost("/admin/clear-data")]
    public async Task<IActionResult> ClearDataConfirm([FromServices] Db db)
    {
        await db.Articles.ExecuteDeleteAsync();
        await db.Persons.ExecuteDeleteAsync();
        return RedirectToAction(nameof(Index));
    }
}