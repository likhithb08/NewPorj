using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace LOCPS.Models
{
    public class LoanProduct
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [StringLength(50)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ProductDescription { get; set; }

        [Required]
        [Range(1000,10000000000)]
        public decimal MinAmount { get; set; }

        [Required]
        [Range(1000, 10000000000)]
        public decimal MaxAmount { get; set; }
        [Required]
        [Range(0,100)]
        public decimal InterestRate { get; set; }
        [Required]
        [Range(1,360)]
        public int MaxTenureMonths { get; set; }
        [Required]
        //Null because it will later be determined by the loan officer that how much fee is deducted
        public decimal ProcessingFee { get; set; }

        //This IsActive fir=eld is required because if the product is inactive the customer should not be able to apply --> soft delete
        public bool IsActive { get; set; } = true; //True because the product is just created

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [ForeignKey(nameof(User))]
        public int CreatedByUserId { get; set; }
        //Referance Navigation Property

        public User User { get; set; } = null!;
        
    }
}
