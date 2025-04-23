using CarRental.Models.CommonProp;
using CarRental.Models.ViewModels;

namespace CarRental.Models
{
    public class Booking : SharedProp
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Brand { get; set; }

        // Make ConfirmationNo optional
        public string? ConfirmationNo { get; set; }

        public string PhoneNumber { get; set; }
        public int BookingID { get; set; }
        public string CarID { get; set; }
        public string FlightName { get; set; }
        public string FlightNumber { get; set; }
        public string Address { get; set; }
        public string Model { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string CardholdersName { get; set; }
        public string CardholdersNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVC { get; set; }

        public Car? Car { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; } = BookingStatus.Pending.ToString();
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}