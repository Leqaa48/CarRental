using CarRental.Data;
using CarRental.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers

{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        public BookingController( AppDbContext context)
        {
            
            _context = context;
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
            }

            double hoursDiff = (endDate.Value - startDate.Value).TotalHours;
            double totalPrice = ((double)car.DailyRate) * hoursDiff;

            var booking = new Booking
            {
                Car = car,
                StartDate = startDate.Value,
                EndDate = endDate.Value
            };

            ViewData["StartDate"] = booking.StartDate.ToString("yyyy-MM-ddTHH:mm");
            ViewData["EndDate"] = booking.EndDate.ToString("yyyy-MM-ddTHH:mm");
            ViewData["TotalPrice"] = totalPrice.ToString("C");

            return View(booking);
        }




        [HttpGet]
        public IActionResult ConfirmRental(string carID, DateTime startDate, DateTime endDate, string firstName, string secondName, string thirdName, string forthName, string email, string phoneNumber)
        {
            // Get the car details from the database using the passed carID
            var car = _context.Cars.FirstOrDefault(c => c.CarID == carID);

            if (car == null)
            {
                // Handle the case where the car ID doesn't exist (e.g., redirect to error page or show an error message)
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction("Index", "Home");
            }

            // Calculate the total price based on the time difference
            var hourlyRate = car.DailyRate;

            var hoursDiff = (endDate - startDate).TotalHours;
            var totalPrice = hourlyRate * (decimal)hoursDiff;

            // Create a booking object
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
                LastName = forthName,
                Email = email,
                PhoneNumber = phoneNumber,
                Status = BookingStatus.Pending.ToString()
            };

            // Display the confirmation page with booking details
            return View("ConfirmRental", booking);
        }


        [HttpPost]
        public IActionResult ConfirmBooking(Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Set initial status if it's not set
                if (string.IsNullOrEmpty(booking.Status))
                {
                    booking.Status = BookingStatus.Pending.ToString();
                }

                // Save to database (assuming you have a DbContext)
                _context.Bookings.Add(booking);
                _context.SaveChanges();

                return RedirectToAction("BookingSuccess"); // Redirect to a success page
            }

            return View("BookingConfirmation", booking); // Re-render if validation fails
        }

        public IActionResult BookingSuccess()
        {
            return View();
        }
    }
}
