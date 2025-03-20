using CarRental.Models.CommonProp;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;  

namespace CarRental.Models
{
    public class Car : SharedProp
    {
        [Required]
        public string CarID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters.")]
        public string Brand { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters.")]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public FuelTypeS FuelType { get; set; }

        [Required]
        public TransmissionS Transmission { get; set; }

        [Required]
        [Range(1, 7, ErrorMessage = "Seats must be between 1 and 7.")]
        public int Seats { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "DailyRate must be between 1 and 1000.")]
        public decimal DailyRate { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; }

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

    public enum TransmissionS
    {
        Automatic, Manual
    }

    public enum FuelTypeS
    {
        Petrol, Diesel, Electric
    }
}
