using System.IO;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Adliance.AspNetCore.Buddy.Template.Razor;
using Bazza.Models;
using Bazza.Services;
using Bazza.ViewModels.AdminPersons;
using Bazza.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
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

    [AllowAnonymous, HttpGet("/register/{accessToken}/labels")]
    public async Task<IActionResult> DownloadLabels([FromServices] LabelsPdfService labelsPdfService, string accessToken)
    {
        return File(await labelsPdfService.BuildPdf(accessToken), "application/pdf", $"Basar Neufelden Labels.pdf");
    }

    [AllowAnonymous, HttpGet("/register/{accessToken}/pdf")]
    public async Task<IActionResult> DownloadPdf([FromServices] ITemplater templater, [FromServices] IPdfer pdfer, [FromServices] EditPersonViewModelFactory factory, string accessToken)
    {
        var viewModel = await factory.Build(accessToken);
        var html = await templater.Render("Pdf", "Person", viewModel);
        var pdf = await pdfer.HtmlToPdf(html, new PdfOptions());
        return File(pdf, "application/pdf", "Basar Neufelden Übersicht.pdf");
    }

    [AllowAnonymous, HttpPost("/register"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromServices] RegisterViewModelFactory factory, RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        await factory.SaveToDatabase(viewModel);
        return View(viewModel);
    }
    
    [AllowAnonymous, HttpGet("/anleitung")]
    public IActionResult Manual()
    {
        return View();
    }

    [AllowAnonymous, Route("error/{status}")]
    public IActionResult Error([FromRoute] int status)
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error is EntityNotFoundException) status = 404;
        return View(status);
    }
}