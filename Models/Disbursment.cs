using LOCPS.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOCPS.Models
{
    public class Disbursment
    {
        [Key]
        public int DisbursmentId { get; set; }
        [ForeignKey(nameof(LoanApplication))]
        public int ApplicationId { get; set; }
        public LoanApplication LoanApplication { get; set; } = null!;
        
        public decimal AmountApproved { get; set; }

        public DateTime? DisbursmentDate { get; set; }
        public DisbursmentMode DisbursmentMode { get; set; } = DisbursmentMode.NEFT;

        [Required]
        public long BankAccountNumber { get; set; }

        [Required]
        public string BankName { get; set; } = string.Empty;

        public string TransactionId { get; set; } = string.Empty;
        public DisbursmentStatus Status { get; set; } = DisbursmentStatus.Pending;

        [ForeignKey(nameof(User))]
        public int ProcessedByUserID { get; set; }
        public User User { get; set; } = null!;

        public string? Notes { get; set; }
      
    }
}
