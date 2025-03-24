using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult RentCar()
        {
            return View();
        }
    }
}
