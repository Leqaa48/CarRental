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
        public IActionResult RentCar(int carId, DateTime? startDate, DateTime? endDate)
        {
            var car = _context.Cars.FirstOrDefault(c => c.CarID == carId.ToString());

            if (car != null && startDate.HasValue && endDate.HasValue)
            {
                var isCarBooked = _context.Bookings.Any(b => b.CarID == carId.ToString() &&
                                                             ((b.StartDate < endDate && b.EndDate > startDate)));

                if (isCarBooked)
                {
                    TempData["ErrorMessage"] = "The car is already booked for this period. Please select a different date or choose another car.";
                    return RedirectToAction("RentCar", new { carId = carId, startDate = startDate, endDate = endDate });
                }

                var hoursDiff = (endDate.Value - startDate.Value).TotalHours; 
                var totalPrice = ((double)car.DailyRate) * hoursDiff; 

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

            return NotFound();
        }




    }
}
