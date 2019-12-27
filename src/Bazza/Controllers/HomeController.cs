using System;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Bazza.Services;
using Bazza.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Bazza.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new IndexViewModel());
        }

        private static readonly object Lock = new object();

        [HttpPost]
        public IActionResult Index([FromServices] Db db, IndexViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            lock (Lock)
            {
                var existingPersonIds = db.Persons.Select(x => x.PersonId).ToList();
                var personId = 1;
                while (existingPersonIds.Contains(personId))
                {
                    personId++;
                }

                db.Persons.Add(new Person
                {
                    PersonId = personId,
                    Address = viewModel.Address,
                    Email = viewModel.Email,
                    Name = viewModel.Name,
                    Phone = viewModel.Phone,
                    CreatedUtc = DateTime.UtcNow
                });

                var articleId = 1;
                foreach (var a in viewModel.Articles)
                {
                    db.Articles.Add(new Article
                    {
                        Name = a.Name,
                        Price = a.Price ?? 0,
                        Size = a.Size,
                        ArticleId = articleId++,
                        PersonId = personId
                    });
                }

                db.SaveChanges();
            }

            viewModel.DisplaySuccess = true;
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
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",$"Basar_Neufelden_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.xlsx");
        }
    }
}