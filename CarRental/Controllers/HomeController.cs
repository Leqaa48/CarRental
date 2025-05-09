using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        public DateTime start;
        public DateTime end;


        public HomeController(ILogger<HomeController> logger, AppDbContext context, EmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        public IActionResult SendWelcomeEmail()
        {
            string emailBody = "<h1>Welcome to our Service</h1><p>We're glad to have you with us!</p>";
            _emailService.SendEmail("ahmadsamen23@gmail.com", "Welcome!", emailBody);
            return View();
        }

        public IActionResult Index(string? confirmationNo)
        {
            if (!string.IsNullOrEmpty(confirmationNo))
            {
                HttpContext.Session.SetString("ConfirmationNo", confirmationNo);
            }

            return View();
        }

        public async Task<IActionResult> CarList(int page = 1, int pageSize = 1000, int? BookingID = null, string? confirmationNo = null)
        {
            ViewBag.BookingID = BookingID;

            ViewBag.confirmationNo = confirmationNo;

            var query = _context.Cars.Where(c => c.Status == "Available");

            // ????? ???????? ?? TempData
            DateTime? startDate = TempData["StartDate"] != null ? Convert.ToDateTime(TempData["StartDate"]) : (DateTime?)null;
            DateTime? endDate = TempData["EndDate"] != null ? Convert.ToDateTime(TempData["EndDate"]) : (DateTime?)null;
         
            if (startDate.HasValue && endDate.HasValue)
            {
                var bookedCarIds = await _context.Bookings
                    .Where(b => b.Status == "Pending" &&
                        ((startDate.Value < b.EndDate && endDate.Value > b.StartDate)
                    ))
                    .Select(b => b.CarID)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(c => !bookedCarIds.Contains(c.CarID));
            }

            int totalCars = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCars / (double)pageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

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

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new Contact());
        }

        [HttpPost]
        public async Task<IActionResult> Contact(Contact model)
        {
            if (ModelState.IsValid)
            {
                _context.Contacts.Add(model);
                await _context.SaveChangesAsync();
                ViewBag.Message = "Your message has been sent successfully!";
                ModelState.Clear();
                return View(new Contact());
            }
            return View(model);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        // Action for loading page
        [HttpGet]
        public IActionResult Loading(string pickupLocation, string dropoffLocation, DateTime startDate, DateTime endDate, string airline, string flightNumber)
        {
            // Store parameters in TempData for use in CarList
            TempData["PickupLocation"] = pickupLocation;
            TempData["DropoffLocation"] = dropoffLocation;
            TempData["StartDate"] = startDate;
            TempData["EndDate"] = endDate;
            TempData["Airline"] = airline;
            TempData["FlightNumber"] = flightNumber;

            start = startDate;
            end= endDate;

            return View();
        }
    }
}