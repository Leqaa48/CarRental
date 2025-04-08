using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CarRental.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public BookingController(AppDbContext context, EmailService emailService, IWebHostEnvironment env)
        {
            _context = context;
            _emailService = emailService;
            _env = env;
        }
        public static string Generate(int length)
        {
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes)
                         .Replace("=", "")
                         .Replace("+", "")
                         .Replace("/", "")
                         .Substring(0, length);
        }
        public IActionResult RentCar(string carId, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now;
            }

            if (!endDate.HasValue)
            {
                endDate = DateTime.Now.AddDays(1);
            }

            if (string.IsNullOrEmpty(carId) || !startDate.HasValue || !endDate.HasValue)
            {
                return BadRequest("Invalid request parameters.");
            }

            var car = _context.Cars.FirstOrDefault(c => c.CarID == carId);

            if (car == null)
            {
                return NotFound();
            }

            bool isCarBooked = _context.Bookings.Any(b =>
                b.CarID == carId &&
                ((b.StartDate < endDate && b.EndDate > startDate))
            );

            if (isCarBooked)
            {
                ViewData["ErrorMessage"] = "The car is already booked for this period. Please select a different date or choose another car.";
                return View();
            }

            double hoursDiff = (endDate.Value - startDate.Value).TotalHours;
            double totalPrice = ((double)car.DailyRate) * hoursDiff;

            var booking = new Booking
            {
                Car = car,
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalPrice = (decimal)totalPrice
            };

            ViewData["StartDate"] = booking.StartDate.ToString("yyyy-MM-ddTHH:mm");
            ViewData["EndDate"] = booking.EndDate.ToString("yyyy-MM-ddTHH:mm");
            ViewData["TotalPrice"] = totalPrice.ToString("C");

            return View(booking);
        }

        [HttpGet] 
        public IActionResult ConfirmRental(string carID, 
     string firstName, string secondName, string thirdName, string LastName,
     string email, string phoneNumber, string flightName, string flightNumber,
     string address, string city, string postCode, string cardholdersName,
     string cardholdersNumber, string expiryDate, string cvc)
        {
            var car = _context.Cars.FirstOrDefault(c => c.CarID == carID);
           DateTime startDate=Convert.ToDateTime(TempData["StartDate"]);
           DateTime endDate=Convert.ToDateTime( TempData["EndDate"]);
            if (car == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction("Index", "Home");
            }

            var hourlyRate = car.DailyRate;
            var hoursDiff = (endDate - startDate).TotalHours;
            var totalPrice = hourlyRate * (decimal)hoursDiff;

            var booking = new Booking
            {
                CarID = carID,
                Car = car,
                StartDate = startDate,
                EndDate = endDate,
                TotalPrice = totalPrice,
                FirstName = firstName,
                SecondName = secondName,
                ThirdName = thirdName,
                LastName = LastName,
                Email = email,
                PhoneNumber = phoneNumber,
                FlightName = flightName,
                FlightNumber = flightNumber,
                Address = address,
                City = city,
                PostCode = postCode,
                CardholdersName = cardholdersName,
                CardholdersNumber = cardholdersNumber,
                ExpiryDate = expiryDate,
                CVC = cvc,
                Status = BookingStatus.Pending.ToString()
            };

            return View("ConfirmRental", booking);
        }

        [HttpPost]
        public IActionResult ConfirmBooking(Booking booking)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(booking.Status))
                {
                    booking.Status = BookingStatus.Pending.ToString();
                }

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                string filePath = Path.Combine(_env.WebRootPath, "emailTemplate.htm");
                string emailTemplate = System.IO.File.ReadAllText(filePath);

                string emailBody = emailTemplate
                    .Replace("{{FirstName}}", booking.FirstName)
                    .Replace("{{SecondName}}", booking.SecondName)
                    .Replace("{{ThirdName}}", booking.ThirdName)
                    .Replace("{{LastName}}", booking.LastName)
                    .Replace("{{Email}}", booking.Email)
                    .Replace("{{PhoneNumber}}", booking.PhoneNumber)
                    .Replace("{{BookingID}}", booking.BookingID.ToString())
                    .Replace("{{CarID}}", booking.CarID)
                    .Replace("{{StartDate}}", booking.StartDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("{{EndDate}}", booking.EndDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("{{TotalPrice}}", booking.TotalPrice.ToString("C"))
                    .Replace("{{FlightName}}", booking.FlightName)
                    .Replace("{{FlightNumber}}", booking.FlightNumber)
                    .Replace("{{Address}}", booking.Address)
                    .Replace("{{City}}", booking.City)
                    .Replace("{{PostCode}}", booking.PostCode)
                    .Replace("{{CardholdersName}}", booking.CardholdersName)
                    .Replace("{{CardholdersNumber}}", booking.CardholdersNumber)
                    .Replace("{{ExpiryDate}}", booking.ExpiryDate)
                    .Replace("{{CVC}}", booking.CVC)
                    .Replace("{{PickupLocation}}", TempData["PickupLocation"].ToString())
                    .Replace("{{DropoffLocation}}", TempData["DropoffLocation"].ToString())
                    .Replace("{{Status}}", booking.Status)
                     .Replace("{{ConfirmationNo}}", Generate(10))

                    ;

               
                


                _emailService.SendEmail(booking.Email, "Your Booking Confirmation", emailBody);

                return RedirectToAction("BookingSuccess");
            }

            return View("ConfirmRental", booking);
        }

        public IActionResult BookingSuccess()
        {
            return View();
        }
    }
}