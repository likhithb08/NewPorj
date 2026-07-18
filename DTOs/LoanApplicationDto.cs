using LOCPS.Enums;
using System.ComponentModel.DataAnnotations;

namespace LOCPS.DTOs;

public class LoanApplicationDto
{
    public int ApplicationId { get; set; }
    public string? ApplicationNumber { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public decimal AnnualIncome { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
}

public class CreateLoanApplicationDto
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1000, 1000000000)]
    public decimal RequestedAmount { get; set; }

    [Required]
    public decimal AnnualIncome { get; set; }

    [Required]
    public string EmploymentType { get; set; } = string.Empty;
}

public class UpdateLoanApplicationStatusDto
{
    [Required]
    public int ApplicationId { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; }
}
