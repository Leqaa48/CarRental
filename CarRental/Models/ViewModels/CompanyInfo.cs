using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.ViewModels
{
    public class CompanyInfo
    {
        [Key]
        public int InfoId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
