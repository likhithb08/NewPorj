using System.ComponentModel.DataAnnotations;

namespace LOCPS.DTOs;

public class LoanProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductDescription { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int MaxTenureMonths { get; set; }
    public decimal ProcessingFee { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLoanProductDto
{
    [Required]
    [StringLength(50)]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ProductDescription { get; set; }

    [Required]
    [Range(1000, 10000000000)]
    public decimal MinAmount { get; set; }

    [Required]
    [Range(1000, 10000000000)]
    public decimal MaxAmount { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal InterestRate { get; set; }

    [Required]
    [Range(1, 360)]
    public int MaxTenureMonths { get; set; }

    [Required]
    public decimal ProcessingFee { get; set; }
}

public class UpdateLoanProductDto
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(50)]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ProductDescription { get; set; }

    [Required]
    [Range(1000, 10000000000)]
    public decimal MinAmount { get; set; }

    [Required]
    [Range(1000, 10000000000)]
    public decimal MaxAmount { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal InterestRate { get; set; }

    [Required]
    [Range(1, 360)]
    public int MaxTenureMonths { get; set; }

    [Required]
    public decimal ProcessingFee { get; set; }

    public bool IsActive { get; set; }
}
