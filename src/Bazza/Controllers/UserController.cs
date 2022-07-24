using System.Security.Claims;
using System.Threading.Tasks;
using Bazza.ViewModels.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Login([FromServices] LoginViewModelFactory factory, LoginViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        viewModel = await factory.Login(viewModel);

        if (!viewModel.Error)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
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