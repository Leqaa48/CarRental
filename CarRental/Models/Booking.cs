﻿using CarRental.Models.CommonProp;
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
        public string PhoneNumber { get; set; }
        //public string PaymentMethod { get; set; }
        public int BookingID { get; set; }
        public string CarID { get; set; }
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


