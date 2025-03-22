using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public string RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }

    }
}
