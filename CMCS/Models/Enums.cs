using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public enum RoleType
    {
        [Display(Name = "Lecturer")]
        Lecturer,
        [Display(Name = "Coordinator")]
        Coordinator,
        [Display(Name = "Manager")]
        Manager,
        [Display(Name = "HR")]
        HR
    }

    public enum ClaimStatus
    {
        [Display(Name = "Submitted")]
        Submitted,
        [Display(Name = "Under Review")]
        UnderReview,
        [Display(Name = "Approved")]
        Approved,
        [Display(Name = "Rejected")]
        Rejected
    }
}