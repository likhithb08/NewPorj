using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LOCPS.Models
{
    public class CreditEvaluation
    {
        [Key]
        public int CreditId { get; set; }

        [ForeignKey(nameof(Application))]
        public int ApplicationId { get; set; }

        public LoanApplication Application { get; set; } = null!;

        [Range(300,900)]
        public int CreditScore { get; set; }
        [Range(5,2)]
        [Required]

        public decimal DebitToIncomeRatio { get; set; }
        [Required]
        public int PaymentHistoryScore { get; set; }

        [Required]
        public decimal ExistingLiabilities { get; set; }


        public CreditRecommendation? CreditRecomendations { get; set; }


        [ForeignKey(nameof(EvaluatedBy))]
        public int? EvaluatedByUserId { get; set; }
        //Evaluation Id to avoid the conflicts in the datatbse creation time
        public User? EvaluatedBy { get; set; }


        public DateTime? EvaluatedDate { get; set; }
        [StringLength(250)]
        public string? Comments { get; set; }
    }
}
