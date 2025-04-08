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

        public HomeController(ILogger<HomeController> logger, AppDbContext context, EmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> SendWelcomeEmail()
        {
            string emailBody = "<h1>Welcome to our Service</h1><p>We're glad to have you with us!</p>";
            _emailService.SendEmail("ahmadsamen23@gmail.com", "Welcome!", emailBody);
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CarList()
        {
            // Fetch the list of cars from the database
            var cars = _context.Cars.ToList();
            return View(cars);
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

            return View();
        }
    }
}