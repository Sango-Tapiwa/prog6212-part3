using CMCS.Models;
using System.ComponentModel.DataAnnotations;

namespace CMCS.ViewModels
{
    public class ClaimSubmissionViewModel
    {
        public int ClaimId { get; set; }

        [Display(Name = "Submission Date")]
        [DataType(DataType.Date)]
        public DateTime SubmissionDate { get; set; } = DateTime.Today;

        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; } = string.Empty;

        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        public List<ClaimItemViewModel> ClaimItems { get; set; } = new List<ClaimItemViewModel>();

        [Display(Name = "Supporting Documents")]
        public List<IFormFile>? Documents { get; set; }

        public decimal TotalAmount { get; set; }
    }

    public class ClaimItemViewModel
    {
        public int ItemId { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Hours Worked")]
        [Range(0.1, 24.0, ErrorMessage = "Hours worked must be between 0.1 and 24")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Display(Name = "Hourly Rate")]
        [Range(0.01, 1000.00, ErrorMessage = "Hourly rate must be between R0.01 and R1000.00")]
        public decimal HourlyRate { get; set; } = 350.00m;

        [Required]
        [Display(Name = "Activity Description")]
        [StringLength(500, ErrorMessage = "Activity description cannot exceed 500 characters")]
        public string ActivityDescription { get; set; } = string.Empty;

        [Display(Name = "Line Total")]
        public decimal LineTotal => HoursWorked * HourlyRate;
    }
}