using System;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Adliance.AspNetCore.Buddy.Template.Razor;
using Bazza.Models.Database;
using Bazza.ViewModels.AdminSales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bazza.Controllers;

public class AdminSalesController : Controller
{
    [Authorize(Roles = Roles.CanManageSales), HttpGet("/admin/sales")]
    public async Task<IActionResult> Sales([FromServices] SalesViewModelFactory factory)
    {
        return View(await factory.Build());
    }

    [Authorize(Roles = Roles.CanManageSales), HttpPost("/admin/sale/create")]
    public async Task<IActionResult> CreateSale([FromServices] Db db)
    {
        var existingSalesIds = await db.Sales.Select(x => x.Id).ToListAsync();
        var newSalesId = 1;
        while (existingSalesIds.Contains(newSalesId)) newSalesId++;

        await db.Sales.AddAsync(new Sale
        {
            CreatedUtc = DateTime.UtcNow,
            Id = newSalesId,
            Username = User.Identity?.Name
        });
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Sale), new { id = newSalesId });
    }

    [Authorize(Roles = Roles.CanManageSales), HttpGet("/admin/sale/{id}")]
    public async Task<IActionResult> Sale([FromServices] SaleViewModelFactory factory, int id)
    {
        return View(await factory.Build(id));
    }

    [Authorize(Roles = Roles.CanManageSales), HttpGet("/admin/sale/{id}/pdf")]
    public async Task<IActionResult> Pdf([FromServices] ITemplater templater, [FromServices] IPdfer pdfer, [FromServices] SaleViewModelFactory factory, int id)
    {
        var viewModel = await factory.Build(id);
        var html = await templater.Render("Pdf", "Sale", viewModel);
        var pdf = await pdfer.HtmlToPdf(html, new PdfOptions());
        return File(pdf, "application/pdf", $"Verkauf {viewModel.Id}.pdf");
    }

    [Authorize(Roles = Roles.CanManageSales), HttpGet("/admin/sale/delete/{id}")]
    public async Task<IActionResult> DeleteSale([FromServices] DeleteSaleViewModelFactory factory, int id)
    {
        return View(await factory.Build(id));
    }

    [Authorize(Roles = Roles.CanManageSales), HttpPost("/admin/sale/delete/{id}")]
    public async Task<IActionResult> DeleteSale([FromServices] DeleteSaleViewModelFactory factory, DeleteSaleViewModel viewModel)
    {
        await factory.UpdateDatabase(viewModel);
        return RedirectToAction(nameof(Sales));
    }

    [Authorize(Roles = Roles.CanManageSales), HttpPost("/admin/sale/{id}/article")]
    public async Task<IActionResult> AddArticle([FromServices] Db db, int id, string article)
    {
        try
        {
            ParseArticle(article, out var personId, out var articleId);
            if (personId <= 0 || articleId <= 0) return BadRequest();
            var articleInDb = await db.Articles.SingleOrDefaultAsync(x => x.PersonId == personId && x.ArticleId == articleId) ?? throw new EntityNotFoundException();
            if (articleInDb.SaleId.HasValue) return BadRequest();

            var sale = await db.Sales.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException();
            articleInDb.SaleId = sale.Id;
            articleInDb.SaleUsername = User.Identity?.Name;
            articleInDb.SaleUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Sale), new { id = sale.Id });
        }
        catch
        {
            return RedirectToAction(nameof(Sale), new { id });
        }
    }

    [Authorize(Roles = Roles.CanManageSales), HttpPost("/admin/sale/{id}/article/{article}")]
    public async Task<IActionResult> RemoveArticle([FromServices] Db db, int id, string article)
    {
        ParseArticle(article, out var personId, out var articleId);
        if (personId <= 0 || articleId <= 0) return BadRequest();
        var articleInDb = await db.Articles.SingleOrDefaultAsync(x => x.PersonId == personId && x.ArticleId == articleId) ?? throw new EntityNotFoundException();

        var sale = await db.Sales.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException();
        if (articleInDb.SaleId == sale.Id)
        {
            articleInDb.SaleId = null;
            articleInDb.SaleUsername = null;
            articleInDb.SaleUtc = null;
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Sale), new { id = sale.Id });
    }

    private void ParseArticle(string article, out int personId, out int articleId)
    {
        personId = 0;
        articleId = 0;
        var ids = article.Split(' ', '_', '-').Where(x => int.TryParse(x, out _)).Select(int.Parse).ToList();
        if (ids.Count != 2) return;
        personId = ids[0];
        articleId = ids[1];
    }
}