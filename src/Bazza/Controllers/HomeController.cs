using System;
using System.Threading;
using System.Threading.Tasks;
using Bazza.Models;
using Bazza.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bazza.Controllers;

public class HomeController : Controller
{
    [AllowAnonymous, HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous, HttpGet("/register")]
    public async Task<IActionResult> Register([FromServices] RegisterViewModelFactory factory, [FromServices] Settings settings)
    {
        if (!settings.RegistrationIsActive) return RedirectToAction(nameof(Index));
        return await Register(factory, (string?)null);
    }

    [AllowAnonymous, HttpGet("/register/{accessToken}")]
    public async Task<IActionResult> Register([FromServices] RegisterViewModelFactory factory, string? accessToken)
    {
        return View(await factory.Fill(accessToken));
    }

    [AllowAnonymous, HttpPost("/register"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromServices] RegisterViewModelFactory factory, RegisterViewModel viewModel)
    {
        // block a little to avoid brute forcing
        Thread.Sleep(new Random().Next(1000, 3000));

        if (!factory.IsCaptchaValid(viewModel)) ModelState.AddModelError(nameof(viewModel.CaptchaResult), "Bitte prüfe das Ergebnis dieser Rechnung.");
        if (!ModelState.IsValid)
        {
            factory.ArmCaptcha(viewModel);
            return View(viewModel);
        }

        await factory.SaveToDatabase(viewModel);
        return View(viewModel);
    }

    [AllowAnonymous, Route("error/{status}")]
    public IActionResult Error([FromRoute] int status)
    {
        return Content($"Error {status}");
    }
}