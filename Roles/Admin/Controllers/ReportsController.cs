using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Services.Interfaces;
using LOCPS.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ILoanApplicationService _loanAppService;
        private readonly ILoanProductService     _productService;
        private readonly IUserService            _userService;

        public ReportsController(
            ILoanApplicationService loanAppService,
            ILoanProductService productService,
            IUserService userService)
        {
            _loanAppService = loanAppService;
            _productService = productService;
            _userService    = userService;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch all applications (large page to get all)
            var query = new PagedQuery { Page = 1, PageSize = 10000 };
            var result = await _loanAppService.SearchAsync(query);
            var apps   = result.Items.ToList();

            var products  = (await _productService.GetAllAsync(false)).ToList();
            var users     = (await _userService.GetAllUsersAsync()).ToList();

            var approved  = apps.Count(a => a.Status == ApplicationStatus.Approved || a.Status == ApplicationStatus.Disbursed || a.Status == ApplicationStatus.Closed);
            var rejected  = apps.Count(a => a.Status == ApplicationStatus.Rejected);
            var pending   = apps.Count(a => a.Status != ApplicationStatus.Approved &&
                                             a.Status != ApplicationStatus.Disbursed &&
                                             a.Status != ApplicationStatus.Closed &&
                                             a.Status != ApplicationStatus.Rejected);

            // Per-product stats
            var productRows = products.Select(p =>
            {
                var productApps = apps.Where(a => a.ProductId == p.ProductId).ToList();
                return new ProductReportRow
                {
                    ProductName        = p.ProductName,
                    TotalApplications  = productApps.Count,
                    ApprovedCount      = productApps.Count(a => a.Status == ApplicationStatus.Approved || a.Status == ApplicationStatus.Disbursed || a.Status == ApplicationStatus.Closed),
                    RejectedCount      = productApps.Count(a => a.Status == ApplicationStatus.Rejected),
                    TotalRequestedAmount = productApps.Sum(a => a.RequestedAmount)
                };
            }).OrderByDescending(r => r.TotalApplications).ToList();

            // Recent 20 applications
            var recentRows = apps.OrderByDescending(a => a.CreatedAt).Take(20).Select(a =>
            {
                var statusClass = a.Status switch
                {
                    ApplicationStatus.Approved or ApplicationStatus.Disbursed or ApplicationStatus.Closed => "badge-status-approved",
                    ApplicationStatus.Rejected => "badge-status-rejected",
                    _ => "badge-status-pending"
                };
                return new RecentApplicationRow
                {
                    ApplicationNumber = a.ApplicationNumber ?? "—",
                    CustomerName      = a.Customer?.FullName ?? "—",
                    ProductName       = a.Product?.ProductName ?? "—",
                    RequestedAmount   = a.RequestedAmount,
                    Status            = a.Status.ToString(),
                    StatusClass       = statusClass,
                    CreatedAt         = a.CreatedAt
                };
            }).ToList();

            var vm = new ReportsViewModel
            {
                TotalApplications = apps.Count,
                ApprovedCount     = approved,
                RejectedCount     = rejected,
                PendingCount      = pending,
                ActiveProducts    = products.Count(p => p.IsActive),
                TotalUsers        = users.Count,
                ProductRows       = productRows,
                RecentApplications = recentRows
            };

            return View(vm);
        }
    }
}
