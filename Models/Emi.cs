using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOCPS.Models
{
    public class Emi
    {
        [Key]
        public int EmiId { get; set; }

        [ForeignKey(nameof(Application))]
        public int ApplicationID { get; set; }

        public LoanApplication Application { get; set; } = null!;

        [Required]
        [Range(1,1000)]
        public int EmiNumber { get; set; }
        public int EmiAmount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; } 
        public decimal? PaidAmount { get; set; }

        public EmiStatus Status { get; set; } = EmiStatus.Pending;

        public decimal PenaltyAmount { get; set; }

    }
}
