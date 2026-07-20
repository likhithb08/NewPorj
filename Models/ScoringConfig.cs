using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOCPS.Models
{
    /// <summary>
    /// Stores the admin-configurable credit scoring engine parameters.
    /// One singleton row (ConfigId = 1) is seeded and kept up-to-date.
    /// </summary>
    public class ScoringConfig
    {
        [Key]
        public int ConfigId { get; set; } = 1;

        // Minimum credit score for loan approval
        [Range(300, 900)]
        public int MinCreditScore { get; set; } = 650;

        // Weight percentages (must sum to 100)
        [Range(0, 100)]
        public int BureauScoreWeight { get; set; } = 35;

        [Range(0, 100)]
        public int DebtToIncomeWeight { get; set; } = 25;

        [Range(0, 100)]
        public int CreditHistoryAgeWeight { get; set; } = 15;

        [Range(0, 100)]
        public int RepaymentConsistencyWeight { get; set; } = 15;

        [Range(0, 100)]
        public int CreditUtilizationWeight { get; set; } = 10;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UpdatedByUser))]
        public int? UpdatedByUserId { get; set; }
        public User? UpdatedByUser { get; set; }
    }
}
