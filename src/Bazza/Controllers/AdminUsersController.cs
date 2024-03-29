﻿using System.Threading.Tasks;
using Bazza.ViewModels.AdminUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bazza.Controllers;

public class AdminUsersController : Controller
{
    [Authorize(Roles = Roles.CanManageAdmin), HttpGet("/admin/users")]
    public async Task<IActionResult> Users([FromServices] UsersViewModelFactory factory)
    {
        return View(await factory.Build());
    }

    [Authorize(Roles = Roles.CanManageAdmin), HttpGet("/admin/user")]
    public async Task<IActionResult> EditUser([FromServices] EditUserViewModelFactory factory, [FromQuery] string? username)
    {
        return View(await factory.Build(username));
    }

    [Authorize(Roles = Roles.CanManageAdmin), HttpPost("/admin/user")]
    public async Task<IActionResult> EditUser([FromServices] EditUserViewModelFactory factory, EditUserViewModel viewModel, [FromQuery] string? username)
    {
        await factory.Validate(ModelState, viewModel, username);
        if (!ModelState.IsValid) return View(viewModel);
        await factory.UpdateDatabase(viewModel, username);
        return RedirectToAction(nameof(Users));
    }

    [Authorize(Roles = Roles.CanManageAdmin), HttpGet("/admin/user/delete")]
    public async Task<IActionResult> DeleteUser([FromServices] DeleteUserViewModelFactory factory, [FromQuery] string username)
    {
        return View(await factory.Build(username));
    }

    [Authorize(Roles = Roles.CanManageAdmin), HttpPost("/admin/user/delete")]
    public async Task<IActionResult> DeleteUser([FromServices] DeleteUserViewModelFactory factory, DeleteUserViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        await factory.UpdateDatabase(viewModel);
        return RedirectToAction(nameof(Users));
    }
}