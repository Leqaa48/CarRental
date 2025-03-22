using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.ViewModels
{
    public class CreateRoleViewModel
    {
            [Required]
            public string RoleName { get; set; }
        
    }
}
