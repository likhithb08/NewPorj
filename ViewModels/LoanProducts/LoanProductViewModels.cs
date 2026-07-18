namespace LOCPS.ViewModels.LoanProducts;

public class LoanProductListViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductDescription { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public decimal? InterestRate { get; set; }
    public int? MaxTenureMonths { get; set; }
    public bool IsActive { get; set; }
}

public class LoanProductCreateViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public string? ProductDescription { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int MaxTenureMonths { get; set; }
    public decimal ProcessingFee { get; set; }
}

public class LoanProductUpdateViewModel : LoanProductCreateViewModel
{
    public int ProductId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LoanProductDetailsViewModel : LoanProductListViewModel
{
    public decimal? ProcessingFee { get; set; }
    public DateTime CreatedAt { get; set; }
}
