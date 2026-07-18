using LOCPS.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOCPS.Models
{
    [Index(nameof(LoanApplication.ApplicationNumber),IsUnique = true)]
    /*
      //Index because the application number should be unique for each application and it is frequently searched field
    and they have to be unique for each application
     */
    public class LoanApplication
    {
        [Key]
        public int ApplicationId { get; set; }
        [Required]
        [StringLength(50)]
        public string? ApplicationNumber { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public User Customer { get; set; } = null!;

        [ForeignKey(nameof(LoanProduct))]
        public int ProductId { get; set; }
        //Null because product is neither creted nor assigned 
        // ! tell the compile to ignore the null value for the time being
        public LoanProduct Product { get; set; } = null!; 
        [Required]
        [Range(1000, 1000000000)]
        public decimal RequestedAmount { get; set; }
        // it is nullable because the amount have to be set after the approval
        public decimal? ApprovedAmount { get; set; }

        [Required]
        public decimal AnnualIncome { get; set; }

        [Required]
        public string EmploymentType { get; set; } = string.Empty;
        //It is submitted because after the application object created it means the application is submitted
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdatedDate { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public int CreatedByUserId { get; set; }
        //Referance Navigation Property

        public User CreatedBy { get; set; } = null!;


    }
}
