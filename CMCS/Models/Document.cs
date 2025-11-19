using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "File Path")]
        public string FilePath { get; set; } = string.Empty;

        [Display(Name = "Upload Date")]
        [DataType(DataType.DateTime)]
        public DateTime UploadDate { get; set; }

        [Display(Name = "File Size (bytes)")]
        public long FileSize { get; set; }

        [Display(Name = "Content Type")]
        public string? ContentType { get; set; }

        // Foreign Key
        [Required]
        public int ClaimId { get; set; }

        // Navigation property
        [ForeignKey("ClaimId")]
        public virtual Claim Claim { get; set; } = null!;
    }
}