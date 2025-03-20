using CarRental.Models;
using CarRental.Models.ViewModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace CarRental.Data
{
    public class AppDbContext : IdentityDbContext <ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

       public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }    
        public DbSet<Payment> Payment { get; set; }

    }
}
