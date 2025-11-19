using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public RoleType Role { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Hourly Rate")]
        [Range(0.01, 1000.00, ErrorMessage = "Hourly rate must be between $0.01 and $1000.00")]
        public decimal HourlyRate { get; set; } = 350.00m;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

        public bool Login(string email, string password)
        {
            // Basic authentication logic (in real implementation, use proper hashing)
            return Email == email && PasswordHash == password;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}