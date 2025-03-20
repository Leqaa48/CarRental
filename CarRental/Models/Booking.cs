using CarRental.Models.CommonProp;
using CarRental.Models.ViewModel;

namespace CarRental.Models
{
    public class Booking : SharedProp
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public int CarID { get; set; }
        public Car? Car { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Cancelled, Completed

    }
    public enum BookingStatus
    {
        Pending, 
        Confirmed, 
        Cancelled, 
        Completed

    }
}
