namespace LOCPS.ViewModels.Reports;

public class ProductReportRow
{
    public string ProductName { get; set; } = string.Empty;
    public int    TotalApplications { get; set; }
    public int    ApprovedCount { get; set; }
    public int    RejectedCount { get; set; }
    public decimal TotalRequestedAmount { get; set; }
}

public class ReportsViewModel
{
    // Summary KPIs
    public decimal TotalDisbursedAmount { get; set; }
    public int     TotalApplications   { get; set; }
    public int     ApprovedCount       { get; set; }
    public int     RejectedCount       { get; set; }
    public int     PendingCount        { get; set; }
    public int     ActiveProducts      { get; set; }
    public int     TotalUsers          { get; set; }

    // Approval rate
    public double ApprovalRatePercent => TotalApplications > 0
        ? Math.Round((double)ApprovedCount / TotalApplications * 100, 1) : 0;

    // Per-product breakdown
    public List<ProductReportRow> ProductRows { get; set; } = new();

    // Recent applications for the table
    public List<RecentApplicationRow> RecentApplications { get; set; } = new();
}

public class RecentApplicationRow
{
    public string ApplicationNumber  { get; set; } = string.Empty;
    public string CustomerName       { get; set; } = string.Empty;
    public string ProductName        { get; set; } = string.Empty;
    public decimal RequestedAmount   { get; set; }
    public string  Status            { get; set; } = string.Empty;
    public string  StatusClass       { get; set; } = string.Empty;
    public DateTime CreatedAt        { get; set; }
}
