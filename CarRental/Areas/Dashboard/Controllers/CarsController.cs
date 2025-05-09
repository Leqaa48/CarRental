﻿using System;
using System.Collections.Generic;
using System.IO;
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
    public class CarsController : Controller
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Cars
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cars.ToListAsync());
        }

        // GET: Dashboard/Cars/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        private void PopulateCarDropdowns()
        {
            ViewBag.StatusList = Enum.GetValues(typeof(CarStatus))
                                     .Cast<CarStatus>()
                                     .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() })
                                     .ToList();
            ViewBag.FuelTypeList = Enum.GetValues(typeof(FuelTypeS))
                                       .Cast<FuelTypeS>()
                                       .Select(f => new SelectListItem { Value = f.ToString(), Text = f.ToString() })
                                       .ToList();

            ViewBag.TransmissionList = Enum.GetValues(typeof(TransmissionS))
                                           .Cast<TransmissionS>()
                                           .Select(t => new SelectListItem { Value = t.ToString(), Text = t.ToString() })
                                           .ToList();
        }

        // GET: Dashboard/Cars/Create
        public IActionResult Create()
        {
            PopulateCarDropdowns();
            return View();
        }

        // POST: Dashboard/Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                if (car.ImageFile != null && car.ImageFile.Length > 0)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/carrentals-master/images");
                        var fileName = Path.GetFileName(car.ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                 
                



                        // Ensure the uploads folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Save the image to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await car.ImageFile.CopyToAsync(fileStream);
                        }

                        // Save the image URL
                        car.Image = $"/carrentals-master/images/{fileName}";
                        car.CompanyLogo = $"/carrentals-master/images/{fileName}";

                        // Save the car to the database
                        _context.Add(car);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while uploading the file.");
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please upload a valid image file.");
                }
            }

            PopulateCarDropdowns();
            return View(car);
        }

        // GET: Dashboard/Cars/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            PopulateCarDropdowns();
            return View(car);
        }

        // POST: Dashboard/Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Car car)
        {
            if (id != car.CarID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing car entity from the database
                    var existingCar = await _context.Cars.FindAsync(id);
                    if (existingCar == null)
                    {
                        return NotFound();
                    }

                    // Update properties from the submitted car
                    existingCar.Brand = car.Brand;
                    existingCar.Model = car.Model;
                    existingCar.Year = car.Year;
                    existingCar.Seats = car.Seats;
                    existingCar.FuelType = car.FuelType;
                    existingCar.Transmission = car.Transmission;
                    existingCar.DailyRate = car.DailyRate;
                    existingCar.Status = car.Status;

                    // Handle image upload logic
                    if (car.ImageFile != null && car.ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        var fileName = Path.GetFileName(car.ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await car.ImageFile.CopyToAsync(fileStream);
                        }

                        existingCar.Image = $"/images/{fileName}";
                    }

                    // Mark the entity as modified
                    _context.Entry(existingCar).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.CarID))
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

            PopulateCarDropdowns();
            return View(car);
        }

        // GET: Dashboard/Cars/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Dashboard/Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(string id)
        {
            return _context.Cars.Any(e => e.CarID == id);
        }
    }
}
