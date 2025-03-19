using CarRental.Models.CommonProp;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Car : SharedProp
    {
        public string CarID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string FuelType { get; set; } // Petrol, Diesel, Electric
        public string Transmission { get; set; } // Automatic, Manual
        public int Seats { get; set; }
        public decimal DailyRate { get; set; }
        public string Status { get; set; } // Available, Booked, Maintenance
        public string Image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

    }
    public enum CarStatus
    {
        Available, 
        Booked, 
        Maintenance

    }
}

