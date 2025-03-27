using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : Controller
    {
        
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            ViewBag.RentedCars = _context.Bookings.Where(c=>c.Status == BookingStatus.Confirmed.ToString()).ToList();
            ViewBag.TotalIncome = _context.Bookings
                .Where(c => c.Status == BookingStatus.Confirmed.ToString())
                .Sum(c => c.TotalPrice);
            return View(_context.Cars.ToList());
        }
    }
}
