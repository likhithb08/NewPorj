using LOCPS.Enums;

namespace LOCPS.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(Roles role, int userId);
}

public class DashboardSummary
{
    public int TotalApplications { get; set; }
    public int PendingKyc { get; set; }
    public int PendingApprovals { get; set; }
    public int PendingDisbursements { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalUsers { get; set; }
}
