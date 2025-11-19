using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        [Display(Name = "Submission Date")]
        [DataType(DataType.DateTime)]
        public DateTime SubmissionDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ClaimStatus Status { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Approved Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "Approval Comments")]
        public string? ApprovalComments { get; set; }

        // Foreign Key
        [Required]
        public int UserId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        public decimal CalculateTotal()
        {
            TotalAmount = ClaimItems?.Sum(item => item.CalculateLineTotal()) ?? 0;
            return TotalAmount;
        }

        public void UpdateStatus(ClaimStatus newStatus, string? comments = null)
        {
            Status = newStatus;
            ApprovalComments = comments;

            if (newStatus == ClaimStatus.Approved || newStatus == ClaimStatus.Rejected)
            {
                ApprovedDate = DateTime.Now;
            }
        }
    }
}