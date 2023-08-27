using System.Threading.Tasks;
using Bazza.Models.Database;
using Bazza.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
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
    
    [Authorize(Roles = Roles.CanManageAdmin), HttpGet("/admin/settings")]
    public async Task<IActionResult> Settings([FromServices] SettingsViewModelFactory factory)
    {
        return View(await factory.Build());
    }

    [Authorize(Roles = Roles.CanManageAdmin), HttpPost("/admin/settings")]
    public async Task<IActionResult> Settings([FromServices] SettingsViewModelFactory factory, SettingsViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        await factory.UpdateDatebase(viewModel);
        return View(viewModel);
    }
    
    [Authorize(Roles = Roles.CanManageAdmin), HttpGet("/admin/clear-data")]
    public IActionResult ClearData([FromServices] Db db)
    {
        return View();
    }

    [Authorize(Roles = Roles.CanManageAdmin), HttpPost("/admin/clear-data")]
    public async Task<IActionResult> ClearDataConfirm([FromServices] Db db)
    {
        await db.Articles.ExecuteDeleteAsync();
        await db.Sales.ExecuteDeleteAsync();
        await db.Persons.ExecuteDeleteAsync();
        return RedirectToAction(nameof(Index));
    }
}