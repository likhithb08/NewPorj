using Microsoft.AspNetCore.Mvc;
using LOCPS.Services.Interfaces;
using LOCPS.Common;
using LOCPS.Constants;
using LOCPS.Enums;
using LOCPS.ViewModels.Dashboard;
using System.Security.Claims;

namespace LOCPS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly ILoanProductService _loanProductService;

        public DashboardController(
            ILoanApplicationService loanApplicationService,
            IUserService userService,
            INotificationService notificationService,
            ILoanProductService loanProductService)
        {
            _loanApplicationService = loanApplicationService;
            _userService = userService;
            _notificationService = notificationService;
            _loanProductService = loanProductService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            var roleIdStr = User.FindFirst(RoleConstants.RoleIdClaimType)?.Value;
            int.TryParse(roleIdStr, out var roleId);
            var role = RoleConstants.GetRoleFromId(roleId);

            // Customer gets a dedicated typed ViewModel
            if (role == Roles.Customer)
            {
                var user = await _userService.GetUserByIdAsync(userId);
                var query = new PagedQuery { Page = 1, PageSize = 10 };
                var applications = await _loanApplicationService.SearchAsync(query, null, userId);
                var notifications = await _notificationService.GetForUserAsync(userId);

                var latestActive = applications.Items
                    .FirstOrDefault(a => a.Status != ApplicationStatus.Closed && a.Status != ApplicationStatus.Rejected);

                var vm = new CustomerDashboardViewModel
                {
                    User = user,
                    Applications = applications,
                    LatestApplication = latestActive,
                    Notifications = notifications.Take(5)
                };
                return View(vm);
            }

            // Staff roles get DashboardViewModel with aggregate counts
            var allQuery = new PagedQuery { Page = 1, PageSize = 8 };
            var allApps = await _loanApplicationService.SearchAsync(allQuery);

            var kycPending = (await _loanApplicationService.SearchAsync(new PagedQuery { Page = 1, PageSize = 1 }, ApplicationStatus.KYCPending)).TotalCount;
            var submitted = (await _loanApplicationService.SearchAsync(new PagedQuery { Page = 1, PageSize = 1 }, ApplicationStatus.Submitted)).TotalCount;
            var underReview = (await _loanApplicationService.SearchAsync(new PagedQuery { Page = 1, PageSize = 1 }, ApplicationStatus.UnderReview)).TotalCount;
            var creditEvaluated = (await _loanApplicationService.SearchAsync(new PagedQuery { Page = 1, PageSize = 1 }, ApplicationStatus.CreditEvaluated)).TotalCount;
            var products = await _loanProductService.GetAllAsync(true);

            var (title, description) = role switch
            {
                Roles.Admin => ("System Administration", "Real-time pipeline analytics, configured scoring weights, and product definitions."),
                Roles.LoanOfficer => ("Loan Officer Command Desk", "Review loan applications, verify compliance documents, and calculate risk credit scores."),
                Roles.UnderWriter => ("Underwriter Review Desk", "Evaluate credit-scored applications, approve or reject, and initiate disbursements."),
                _ => ("Dashboard", "Welcome to LOCPS.")
            };

            var dashVm = new DashboardViewModel
            {
                RoleTitle = title,
                RoleDescription = description,
                TotalApplications = allApps.TotalCount,
                PendingKyc = kycPending + submitted,
                PendingApprovals = underReview + creditEvaluated,
                ActiveProducts = products.Count(),
                RecentApplications = allApps.Items.Select(a => new DashboardApplicationRowViewModel
                {
                    ApplicationId = a.ApplicationId,
                    ApplicationNumber = a.ApplicationNumber,
                    CustomerName = a.Customer?.FullName ?? "—",
                    ProductName = a.Product?.ProductName ?? "—",
                    RequestedAmount = a.RequestedAmount,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                }).ToList()
            };

            return View(dashVm);
        }
    }
}
