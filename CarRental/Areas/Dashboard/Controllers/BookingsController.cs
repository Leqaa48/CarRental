using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;
using CarRental.Models;

namespace CarRental.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Bookings
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Bookings.Include(b => b.Car);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Pending()
        {
            var appDbContext = _context.Bookings
                .Where(x => x.Status == BookingStatus.Pending.ToString())
                .Include(b => b.Car);  // Ensure Car is included
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Cancelled()
        {
            var appDbContext = _context.Bookings
                .Where(x => x.Status == BookingStatus.Cancelled.ToString())
                .Include(b => b.Car);  // Ensure Car is included
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Completed()
        {
            var appDbContext = _context.Bookings
                .Where(x => x.Status == BookingStatus.Completed.ToString())
                .Include(b => b.Car);  // Ensure Car is included
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Confirmed()
        {
            var appDbContext = _context.Bookings
                .Where(x => x.Status == BookingStatus.Confirmed.ToString())
                .Include(b => b.Car);  // Ensure Car is included
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dashboard/Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Dashboard/Bookings/Create
        public IActionResult Create()
        {
            ViewData["CarID"] = new SelectList(_context.Cars, "CarID", "CarID");
            return View();
        }

        // POST: Dashboard/Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarID"] = new SelectList(_context.Cars, "CarID", "CarID", booking.CarID);
            return View(booking);
        }

        // GET: Dashboard/Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewBag.BookingStatus = Enum.GetValues(typeof(BookingStatus))
                                    .Cast<BookingStatus>()
                                    .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() })
                                    .ToList();
            ViewData["CarID"] = new SelectList(_context.Cars, "CarID", "CarID", booking.CarID);
            return View(booking);
        }

        // POST: Dashboard/Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarID"] = new SelectList(_context.Cars, "CarID", "CarID", booking.CarID);
            return View(booking);
        }

        // GET: Dashboard/Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Dashboard/Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }
    }
}
