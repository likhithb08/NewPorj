using LOCPS.Enums;

namespace LOCPS.ViewModels.LoanApplications;

public class LoanApplicationListViewModel
{
    public int ApplicationId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public int TenureMonths { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LoanApplicationDetailsViewModel : LoanApplicationListViewModel
{
    public decimal? ApprovedAmount { get; set; }
    public decimal AnnualIncome { get; set; }
    public string EmployeementType { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
}

public class LoanApplicationCreateViewModel
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal AnnualIncome { get; set; }
    public string EmployeementType { get; set; } = string.Empty;
    public int TenureMonths { get; set; } = 12;
}

public class LoanApplicationUpdateViewModel : LoanApplicationCreateViewModel
{
    public int ApplicationId { get; set; }
    public ApplicationStatus Status { get; set; }
}

public class LoanApplicationListPageViewModel
{
    public IReadOnlyList<LoanApplicationListViewModel> Items { get; set; } = Array.Empty<LoanApplicationListViewModel>();
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public string? Search { get; set; }
    public ApplicationStatus? StatusFilter { get; set; }
}
