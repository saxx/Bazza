using Microsoft.AspNetCore.Mvc;

namespace Bazza.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}