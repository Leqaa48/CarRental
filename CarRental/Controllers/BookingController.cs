using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult AllBookings()
        {
            var bookings = _context.Bookings.ToList(); // Booking NOT BookingViewModel
            return View(bookings);
        }

        public IActionResult RentCar(string carId, DateTime? startDate, DateTime? endDate, int? BookingID = null,string? confirmationNo=null)
        {
            ViewData["pickupLocation"] = TempData["PickupLocation"]?.ToString() ?? "Amman";
            ViewData["dropoffLocation"] = TempData["DropoffLocation"]?.ToString() ?? "Amman";
            if (!string.IsNullOrEmpty(confirmationNo))
            {
                HttpContext.Session.SetString("ConfirmationNo", confirmationNo);
            }

            if (BookingID == null)
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

                double hoursDiff = (endDate.Value - startDate.Value).TotalDays;
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
            else
            {
                var car = _context.Cars.FirstOrDefault(c => c.CarID == carId);

                if (car == null)
                {
                    return NotFound();
                }

                var result = _context.Bookings.FirstOrDefault(b => b.BookingID == BookingID);
                if (result == null)
                {
                    return NotFound();
                }

                result.Car = car;
                ViewData["StartDate"] = result.StartDate.ToString("yyyy-MM-ddTHH:mm");
                ViewData["EndDate"] = result.EndDate.ToString("yyyy-MM-ddTHH:mm");
                ViewData["TotalPrice"] = result.TotalPrice.ToString("C");

                ViewBag.BookingID = BookingID;

                ViewBag.confirmationNo = confirmationNo;

                // تأكد من عدم تغيير ConfirmationNo
                result.ConfirmationNo = confirmationNo;

                return View(result);
            }
        }
        [HttpGet]

        [HttpGet]
        public IActionResult ConfirmRental(string carID, string Brand, string StartDate, string EndDate,
    string firstName, string secondName, string thirdName, string LastName, decimal TotalPrice,
    string email, string phoneNumber, string flightName, string flightNumber,
    string address, string city, string postCode, string cardholdersName,
    string cardholdersNumber, string expiryDate, string cvc,string pickupLocation,string dropoffLocation, int? BookingID = null)

        {
            var car = _context.Cars.FirstOrDefault(c => c.CarID == carID);

            if (car == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction("Index", "Home");
            }

            // الحصول على التواريخ الحالية من الحجز إذا كان موجودًا
            Booking existingBooking = null;
            if (BookingID != null)
            {
                existingBooking = _context.Bookings.FirstOrDefault(b => b.BookingID == BookingID);
            }

            DateTime startDate = existingBooking?.StartDate ?? Convert.ToDateTime(StartDate);
            DateTime endDate = existingBooking?.EndDate ?? Convert.ToDateTime(EndDate);
            int daysDifference = (endDate - startDate).Days;
            ViewBag.totalPrice = car.DailyRate * daysDifference;
            ViewBag.Brand = car.Brand;
            ViewBag.model = car.Model;
            //var hourlyRate = car.DailyRate;
            //var hoursDiff = (endDate - startDate).TotalDays;
            //var totalPrice = hourlyRate * (decimal)hoursDiff;

            var booking = new Booking
            {
                BookingID = BookingID ?? 0,
                CarID = carID,
                Brand = Brand,
                Car = car,
                StartDate = startDate,
                EndDate = endDate,
                TotalPrice = TotalPrice,
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
                Model = car.Model,
                Status = BookingStatus.Pending.ToString()



            };

            TempData["PickupLocation"] = pickupLocation;
            TempData["DropoffLocation"] = dropoffLocation;

            return View("ConfirmRental", booking);
        }

        [HttpPost]
        
        public IActionResult ConfirmBooking(Booking booking)
        {
            bool isSave = false;
            string confirmationNo;

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(booking.Status))
                {
                    booking.Status = BookingStatus.Pending.ToString();
                }

                if (HttpContext.Session.GetString("ConfirmationNo")!=null)
                {
                    TempData["booking"] = _context.Bookings.AsNoTracking().FirstOrDefault(b => b.ConfirmationNo== HttpContext.Session.GetString("ConfirmationNo"));
                    booking.ConfirmationNo = HttpContext.Session.GetString("ConfirmationNo");
                    
                    _context.Bookings.Update(booking);
                    _context.SaveChanges();
                    HttpContext.Session.Remove("ConfirmationNo");

                }
                else
                {
                    isSave = true;
                    confirmationNo = Generate(10);
                    TempData["ConfirmationNo"] = confirmationNo;
                    booking.ConfirmationNo = confirmationNo;
                    _context.Bookings.Add(booking);
                    _context.SaveChanges();
                }

                string filePath = Path.Combine(_env.WebRootPath, "emailTemplate.htm");
                string emailTemplate = System.IO.File.ReadAllText(filePath);

                string pickupLocation = TempData["PickupLocation"]?.ToString() ?? "Amman";
                string dropoffLocation = TempData["DropoffLocation"]?.ToString() ?? "Amman";
                confirmationNo = TempData["ConfirmationNo"]?.ToString() ?? "No Confirmation";

                string emailBody = emailTemplate
                    .Replace("{{FirstName}}", booking.FirstName ?? "First Name")
                    .Replace("{{SecondName}}", booking.SecondName ?? "Second Name")
                    .Replace("{{ThirdName}}", booking.ThirdName ?? "Third Name")
                    .Replace("{{LastName}}", booking.LastName ?? "Last Name")
                    .Replace("{{Email}}", booking.Email ?? "No Email")
                    .Replace("{{PhoneNumber}}", booking.PhoneNumber ?? "No Phone Number")
                    .Replace("{{BookingID}}", booking.BookingID.ToString())
                    .Replace("{{CarID}}", booking.CarID ?? "Unknown Car")
                    .Replace("{{Brand}}", booking.Brand ?? "Unknown Brand")
                    .Replace("{{StartDate}}", booking.StartDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("{{EndDate}}", booking.EndDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("{{TotalPrice}}", booking.TotalPrice.ToString("C"))
                    .Replace("{{FlightName}}", booking.FlightName ?? "No Flight Name")
                    .Replace("{{FlightNumber}}", booking.FlightNumber ?? "No Flight Number")
                    .Replace("{{Address}}", booking.Address ?? "No Address")
                    .Replace("{{City}}", booking.City ?? "No City")
                    .Replace("{{PostCode}}", booking.PostCode ?? "No Post Code")
                    .Replace("{{CardholdersName}}", booking.CardholdersName ?? "No Cardholder Name")
                    .Replace("{{CardholdersNumber}}", booking.CardholdersNumber ?? "No Card Number")
                    .Replace("{{ExpiryDate}}", booking.ExpiryDate ?? "No Expiry Date")
                    .Replace("{{CVC}}", booking.CVC ?? "No CVC")
                    .Replace("{{PickupLocation}}", pickupLocation)
                    .Replace("{{DropoffLocation}}", dropoffLocation)
                    .Replace("{{Status}}", booking.Status)
                    .Replace("{{Model}}", booking.Model)

                    .Replace("{{ConfirmationNo}}", confirmationNo);

                if (!isSave)
                {
                    Booking? previousBooking = TempData["booking"] as Booking;
                    if (previousBooking != null)
                    {
                        var diff = GetDifferences<Booking>(booking, previousBooking);
                        emailBody = emailBody.Replace("{{Changes}}", string.Join("<br/>", diff));
                    }
                }
                else
                {
                    emailBody = emailBody.Replace("{{Changes}}", "No changes, this is a new booking.");
                }


                _emailService.SendEmail(booking.Email, isSave ? "Your Booking Confirmation" : "Your Booking Updated", emailBody);
                TempData["booking"] = null;

                return RedirectToAction("BookingSuccess", new { booking.CarID, booking.BookingID, ConfirmationNo = confirmationNo });
            }

            return View("ConfirmRental", booking);
        }

        public IActionResult BookingSuccess(string CarID, long BookingID,string ConfirmationNo)
        {
            if (HttpContext.Session.GetString("ConfirmationNo") != null)
            {
                ViewBag.ConfirmationNo = HttpContext.Session.GetString("ConfirmationNo");            }
            else
            {
                ViewBag.ConfirmationNo = ConfirmationNo;

            }

            ViewBag.CarID = CarID;
            ViewBag.BookingID = BookingID;
            return View();
        }

        [HttpPost]
        public IActionResult CancelBooking(long BookingID)
        {
            bool isCancelled = false;
            string confirmationNo;

            var result = _context.Bookings.FirstOrDefault(x => x.BookingID == BookingID);
            confirmationNo = TempData["ConfirmationNo"]?.ToString() ?? "No Confirmation";

            if (result != null)
            {
                isCancelled = true;
                _context.Bookings.Remove(result);
                _context.SaveChanges();
            }

            string filePath = Path.Combine(_env.WebRootPath, "CancleTemplate.htm");
            string emailTemplate = System.IO.File.ReadAllText(filePath);

            string emailBody = emailTemplate
                .Replace("{{FirstName}}", result?.FirstName ?? "First Name")
                .Replace("{{SecondName}}", result?.SecondName ?? "Second Name")
                .Replace("{{ThirdName}}", result?.ThirdName ?? "Third Name")
                .Replace("{{LastName}}", result?.LastName ?? "Last Name")
                .Replace("{{Email}}", result?.Email ?? "No Email")
                .Replace("{{PhoneNumber}}", result?.PhoneNumber ?? "No Phone Number")
                .Replace("{{BookingID}}", result?.BookingID.ToString() ?? "0")
                .Replace("{{CarID}}", result?.CarID ?? "Unknown Car")
                .Replace("{{StartDate}}", result?.StartDate.ToString("yyyy-MM-dd HH:mm") ?? "")
                .Replace("{{EndDate}}", result?.EndDate.ToString("yyyy-MM-dd HH:mm") ?? "")
                .Replace("{{TotalPrice}}", result?.TotalPrice.ToString("C") ?? "0.00")
                .Replace("{{FlightName}}", result?.FlightName ?? "No Flight Name")
                .Replace("{{FlightNumber}}", result?.FlightNumber ?? "No Flight Number")
                .Replace("{{Address}}", result?.Address ?? "No Address")
                .Replace("{{City}}", result?.City ?? "No City")
                .Replace("{{PostCode}}", result?.PostCode ?? "No Post Code")
                .Replace("{{CardholdersName}}", result?.CardholdersName ?? "No Cardholder Name")
                .Replace("{{CardholdersNumber}}", result?.CardholdersNumber ?? "No Card Number")
                .Replace("{{ExpiryDate}}", result?.ExpiryDate ?? "No Expiry Date")
                .Replace("{{CVC}}", result?.CVC ?? "No CVC")
                .Replace("{{Status}}", result?.Status ?? "Cancelled")
                .Replace("{{ConfirmationNo}}", confirmationNo);

            if (isCancelled)
            {
                emailBody = emailBody.Replace("{{CancellationStatus}}", "Your booking has been cancelled successfully.");
            }
            else
            {
                emailBody = emailBody.Replace("{{CancellationStatus}}", "No booking found to cancel.");
            }

            _emailService.SendEmail(result?.Email ?? "noemail@example.com", "Your Booking Is Cancele", emailBody);

            return View();
        }

        public static List<string> GetDifferences<T>(T obj1, T obj2)
        {
            var differences = new List<string>();
            var type = typeof(T);

            foreach (var prop in type.GetProperties())
            {
                var val1 = prop.GetValue(obj1);
                var val2 = prop.GetValue(obj2);

                if ((val1 == null && val2 != null) ||
                    (val1 != null && !val1.Equals(val2)))
                {
                    differences.Add($"{prop.Name}: '{val1}' vs '{val2}'");
                }
            }

            return differences;
        }
    }
}
