using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.SymbolStore;
using LOCPS.Enums;
namespace LOCPS.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [ForeignKey(nameof(Application))]
        public int ApplicationId { get; set; }
        public LoanApplication Application { get; set; } = null!; //nav property = Application

        [ForeignKey(nameof(Kyc))]
        public int? KycId { get; set; }
        public Kyc? Kyc { get; set; } = null!;

        [Required]
        public DocumentType DocumentType { get; set; }

        [StringLength(50)]
        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Customer))]
        public int UploadedByUserId { get; set; }
        public User Customer { get; set; } = null!;

        public DocumentStatus DocumentStatus { get; set; } = DocumentStatus.Pending;

        [StringLength(500)]
        public string? Remarks { get; set; }

        [ForeignKey(nameof(Verifier))]
        public int VerifiedByUserId { get; set; }
        public User Verifier { get; set; } = null!;

    }
}
