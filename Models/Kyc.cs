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
        [StringLength(15)]
        //This is a regular expression that checks the pattern as digits in adhaar card
        [RegularExpression(@"^\d{15}$")]
        public string AdhaarNumber { get; set; }

        [Required]
        [StringLength(12)]
        //This is a regular expression that cehcks the pattern of PAN Card
        [RegularExpression(@"^[A-Z{5}[0-9]{4}[A-Z]$")]
        public string PanNumber { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]

        public Gender? Gender { get; set; }
        

        [StringLength(250)]
        public string? AddressProof { get; set; }

        [Required]
        [StringLength(200)]
        public string IdentityProof { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string IncomeProof { get; set; } = string.Empty;

        public KycStatus VerificationStatus { get; set; } = KycStatus.Pending;

        [ForeignKey(nameof(Verification))]
        public int? VerifiedByUserId { get; set; }
        //Verify Id to avoid the conflicts in the datatbse creation time
        public User? Verification { get; set; }

        public bool IsActive { get; set; } = true;


        public DateTime? VerifiedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }
}
