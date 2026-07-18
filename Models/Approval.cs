using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LOCPS.Enums;

namespace LOCPS.Models
{
    public class Approval
    {
        [Key]
        public int ApprovalId { get; set; }

        [ForeignKey(nameof(Application))]
        public int ApplicationId { get; set; }
        public LoanApplication Application { get; set; } = null!;

        public long? ApprovedAmount { get; set; }

        public int ApprovedTenureMonths { get; set; }

        public decimal ApprovedInterestRate { get; set; }

        public ApprovalStatus? ApprovalStatus { get; set; }

        [ForeignKey(nameof(Approver))]
        public int ApprovedByUserId { get; set; }
        public User Approver { get; set; } = null!;

        public DateTime ApprovalDate { get; set; }

        public string? RejectionReason { get; set; }

        public string Comments { get; set; } = string.Empty;
    }
}
