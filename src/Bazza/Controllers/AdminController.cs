using System;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Bazza.Services;
using Bazza.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Bazza.Controllers;

public class AdminController : Controller
{
    [HttpGet("/admin")]
    public async Task<IActionResult> Index([FromServices] IndexViewModelFactory factory)
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

    [HttpPost("/admin/clear-data")]
    public async Task<IActionResult> ClearData([FromServices] Db db)
    {
        db.Articles.RemoveRange(db.Articles);
        db.Persons.RemoveRange(db.Persons);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}