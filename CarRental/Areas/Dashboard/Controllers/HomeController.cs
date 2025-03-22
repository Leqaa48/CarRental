using Microsoft.AspNetCore.Mvc;

namespace CarRental.Areas.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        [Area("Dashboard")]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Cars", new { area = "Dashboard" });
        }
    }
}
