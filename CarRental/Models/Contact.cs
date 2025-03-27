using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Contact
    {
        [Key]
        public int MessageId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }
}