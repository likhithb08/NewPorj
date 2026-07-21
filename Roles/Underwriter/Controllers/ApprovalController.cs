using Microsoft.AspNetCore.Mvc;
using LOCPS.Services.Interfaces;
using LOCPS.Models;
using LOCPS.Enums;
using LOCPS.Common;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace LOCPS.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly IKycService _kycService;
        private readonly IDocumentService _documentService;
        private readonly ICreditEvaluationService _creditService;
        private readonly IApprovalService _approvalService;
        private readonly IAuditLogService _auditLogService;

        public ApprovalController(
            ILoanApplicationService loanApplicationService,
            IKycService kycService,
            IDocumentService documentService,
            ICreditEvaluationService creditService,
            IApprovalService approvalService,
            IAuditLogService auditLogService)
        {
            _loanApplicationService = loanApplicationService;
            _kycService = kycService;
            _documentService = documentService;
            _creditService = creditService;
            _approvalService = approvalService;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, ApplicationStatus? status = null)
        {
            // Default: show applications awaiting underwriter decision
            var query = new PagedQuery { Page = page, PageSize = pageSize };
            var result = await _loanApplicationService.SearchAsync(query, status);

            // If no status filter applied, show CreditEvaluated + UnderReview combined (pending decision)
            if (!status.HasValue)
            {
                var creditEvaluated = await _loanApplicationService.SearchAsync(new PagedQuery { Page = 1, PageSize = 50 }, ApplicationStatus.CreditEvaluated);
                var underReview = await _loanApplicationService.SearchAsync(new PagedQuery { Page = 1, PageSize = 50 }, ApplicationStatus.UnderReview);
                var allPending = creditEvaluated.Items.Concat(underReview.Items)
                    .OrderByDescending(a => a.CreatedAt).ToList();

                result = new PagedResult<LoanApplication>
                {
                    Items = allPending.Skip((Math.Max(page, 1) - 1) * pageSize).Take(pageSize).ToList(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = allPending.Count
                };
            }

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
                return RedirectToAction(nameof(Index));

            var kyc = await _kycService.GetByApplicationIdAsync(id);
            var documents = await _documentService.GetByApplicationIdAsync(id);
            var creditEval = await _creditService.GetByApplicationIdAsync(id);
            var auditLogs = await _auditLogService.GetAuditLogsForApplicationAsync(id);

            ViewBag.Kyc = kyc;
            ViewBag.Documents = documents;
            ViewBag.CreditEvaluation = creditEval;
            ViewBag.AuditLogs = auditLogs;

            return View(application);
        }

        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            var approval = await _approvalService.GetByApplicationIdAsync(id);
            if (approval != null && approval.ApprovalStatus == ApprovalStatus.Approved)
            {
                return View(approval);
            }
            return RedirectToAction(nameof(Review), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, decimal approvedAmount, int approvedTenureMonths, decimal approvedInterestRate, string comments)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var approverUserId = int.TryParse(userIdStr, out var uid) ? uid : 3;

                var approval = await _approvalService.ApproveLoanAsync(id, approverUserId, approvedAmount, approvedTenureMonths, approvedInterestRate, comments);
                TempData["Success"] = "Loan application approved successfully.";
                return View(approval);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Review), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Reject(int id)
        {
            var approval = await _approvalService.GetByApplicationIdAsync(id);
            if (approval != null && approval.ApprovalStatus == ApprovalStatus.Rejected)
            {
                return View(approval);
            }
            return RedirectToAction(nameof(Review), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string rejectionReason, string comments)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var approverUserId = int.TryParse(userIdStr, out var uid) ? uid : 3;

                var approval = await _approvalService.RejectLoanAsync(id, approverUserId, rejectionReason, comments);
                TempData["Success"] = "Loan application rejected.";
                return View(approval);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Review), new { id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendBack(int id, string remarks)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var approverUserId = int.TryParse(userIdStr, out var uid) ? uid : 3;

                if (string.IsNullOrWhiteSpace(remarks))
                {
                    TempData["Error"] = "Remarks are required to request re-evaluation.";
                    return RedirectToAction(nameof(Review), new { id });
                }

                await _approvalService.SendBackToLoanOfficerAsync(id, approverUserId, remarks);
                TempData["Success"] = "Application sent back to Loan Officer for re-evaluation.";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Review), new { id });
            }
        }
    }
}
