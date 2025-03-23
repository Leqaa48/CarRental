using System.Diagnostics;
using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CarList(DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 6)
        {
            
            var query = _context.Cars.Where(c => c.Status == "Available");

           
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(c =>
                    (c.AvailableFrom <= startDate.Value && c.AvailableTo >= endDate.Value)
                    || (c.AvailableFrom >= startDate.Value && c.AvailableFrom <= endDate.Value)
                    || (c.AvailableTo >= startDate.Value && c.AvailableTo <= endDate.Value)
                );
            }

            int totalCars = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCars / (double)pageSize);

            var paginatedCars = await query
                .OrderBy(c => c.Brand) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-ddTHH:mm");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-ddTHH:mm");

            return View(paginatedCars);
        }





        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
