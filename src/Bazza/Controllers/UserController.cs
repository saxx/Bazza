using System.Security.Claims;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Bazza.ViewModels.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bazza.Controllers;

public class UserController : Controller
{
    [AllowAnonymous, HttpGet("/user/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Redirect("~/");
    }

    [AllowAnonymous, HttpGet("/user/login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction(nameof(AdminController.Index), "Admin");
        return View(new LoginViewModel());
    }

    [AllowAnonymous, HttpPost("/user/login")]
    public async Task<IActionResult> Login([FromServices] LoginViewModelFactory factory, [FromServices] Db db, LoginViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        viewModel = await factory.Login(viewModel);

        if (!viewModel.Error)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            var user = await db.Users.SingleAsync(x => x.Username.ToLower() == viewModel.Username.ToLower());
            if (user.CanManageAdmin) identity.AddClaim(new Claim(ClaimTypes.Role, Roles.CanManageAdmin));
            if (user.CanManagePersons) identity.AddClaim(new Claim(ClaimTypes.Role, Roles.CanManagePersons));
            if (user.CanManageSales) identity.AddClaim(new Claim(ClaimTypes.Role, Roles.CanManageSales));
            identity.AddClaim(new Claim(ClaimTypes.Name, viewModel.Username));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (viewModel.RequiresPasswordReset) return RedirectToAction(nameof(ResetPassword), new { username = viewModel.Username });
            return RedirectToAction(nameof(AdminController.Index), "Admin");
        }

        return View(viewModel);
    }

    [HttpGet("/user/reset-password")]
    public IActionResult ResetPassword(ResetPasswordViewModel viewModel)
    {
        return View(viewModel);
    }

    [HttpPost("/user/reset-password")]
    public async Task<IActionResult> ResetPassword([FromServices] ResetPasswordViewModelFactory factory, ResetPasswordViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        await factory.ResetPassword(viewModel);
        return View(viewModel);
    }
}