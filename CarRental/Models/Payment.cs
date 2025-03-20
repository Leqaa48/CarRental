using CarRental.Models.CommonProp;
using CarRental.Models.ViewModels;

namespace CarRental.Models
{
    public class Payment : SharedProp
    {
        public int PaymentID { get; set; }
        public int BookingID { get; set; }
        public Booking? Booking { get; set; }
        public int UserID { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } // Pending, Completed, Failed
        public string TransactionID { get; set; }

    }
    public enum PaymentStatus
    {
        Pending, 
        Completed, 
        Failed

    }
}
