using LOCPS.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LOCPS.Models
{
    public class Kyc
    {
        [Key]
        public int KycId { get; set; }
        //Foreign key for the Application id from LoanApplication Table
        [ForeignKey(nameof(Application))]
        public int ApplicationId { get; set; }
        public LoanApplication Application { get; set; } = null!;

        [Required]
        [StringLength(12)]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar must be 12 digits.")]
        public string AadhaarNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]$", ErrorMessage = "Invalid PAN format.")]
        public string PanNumber { get; set; } = string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; } = LOCPS.Enums.Gender.Male;
        

        [StringLength(250)]
        public string? AddressProof { get; set; }

        [Required]
        [StringLength(200)]
        public string IdentityProof { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string IncomeProof { get; set; } = string.Empty;

        public KycStatus VerificationStatus { get; set; } = KycStatus.Pending;

        [StringLength(500)]
        public string? Remarks { get; set; }

        [ForeignKey(nameof(Verification))]
        public int? VerifiedByUserId { get; set; }
        //Verify Id to avoid the conflicts in the datatbse creation time
        public User? Verification { get; set; }

        public bool IsActive { get; set; } = true;


        public DateTime? VerifiedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }
}
