using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;

namespace LOCPS.ViewModels.Dashboard;

public class DashboardViewModel
{
    public string RoleTitle { get; set; } = string.Empty;
    public string RoleDescription { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public int PendingKyc { get; set; }
    public int PendingApprovals { get; set; }
    public int PendingDisbursements { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalUsers { get; set; }
    public IReadOnlyList<DashboardApplicationRowViewModel> RecentApplications { get; set; } = Array.Empty<DashboardApplicationRowViewModel>();
}

public class DashboardApplicationRowViewModel
{
    public int ApplicationId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerDashboardViewModel
{
    public User User { get; set; } = null!;
    public LoanApplication? LatestApplication { get; set; }
    public PagedResult<LoanApplication> Applications { get; set; } = new();
    public IEnumerable<Notification> Notifications { get; set; } = Enumerable.Empty<Notification>();
}
