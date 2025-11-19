using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class ClaimItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Hours Worked")]
        [Range(0.1, 24.0, ErrorMessage = "Hours worked must be between 0.1 and 24")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Display(Name = "Hourly Rate")]
        [Range(0.01, 1000.00, ErrorMessage = "Hourly rate must be between $0.01 and $1000.00")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(8,2)")]
        public decimal HourlyRate { get; set; }

        [Required]
        [Display(Name = "Activity Description")]
        [StringLength(500, ErrorMessage = "Activity description cannot exceed 500 characters")]
        public string ActivityDescription { get; set; } = string.Empty;

        // Foreign Key
        [Required]
        public int ClaimId { get; set; }

        // Navigation property
        [ForeignKey("ClaimId")]
        public virtual Claim Claim { get; set; } = null!;

        [Display(Name = "Line Total")]
        [DataType(DataType.Currency)]
        public decimal LineTotal => CalculateLineTotal();

        public decimal CalculateLineTotal()
        {
            return HoursWorked * HourlyRate;
        }
    }
}