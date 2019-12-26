using Bazza.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace Bazza.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Index(IndexViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            
            return View();
        }
    }
}