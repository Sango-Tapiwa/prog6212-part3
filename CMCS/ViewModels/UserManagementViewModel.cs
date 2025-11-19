using CMCS.Models;
using System.ComponentModel.DataAnnotations;

namespace CMCS.ViewModels
{
    public class UserManagementViewModel
    {
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public RoleType Role { get; set; }

        [Required]
        [Display(Name = "Hourly Rate")]
        [Range(0, 1000.00, ErrorMessage = "Hourly rate must be between R0 and R1000.00")]
        public decimal HourlyRate { get; set; } = 350.00m;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
