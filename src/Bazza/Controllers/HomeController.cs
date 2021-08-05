using System;
using System.Threading.Tasks;
using Bazza.Services;
using Bazza.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Bazza.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet("/register")]
        public async Task<IActionResult> Register([FromServices] RegisterViewModelFactory factory)
        {
            return await Register(factory, (string)null);
        }
        
        [HttpGet("/register/{accessToken}")]
        public async Task<IActionResult> Register([FromServices] RegisterViewModelFactory factory, string accessToken)
        {
            return View(await factory.Fill(accessToken));
        }
        
        [HttpPost("/register"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromServices] RegisterViewModelFactory factory, RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await factory.SaveToDatabase(viewModel);
            return View(viewModel);
        }

        [HttpGet("/download/{token}")]
        public async Task<IActionResult> Download([FromServices] ExcelExportService excel, [FromServices] IConfiguration configuration, string token)
        {
            if (token != configuration.GetValue<string>("AdminToken"))
            {
                return NotFound();
            }

            var bytes = await excel.CreateExcelFile();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Basar_Neufelden_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.xlsx");
        }
    }
}