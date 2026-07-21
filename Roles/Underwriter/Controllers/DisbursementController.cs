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
    public class DisbursementController : Controller
    {
        private readonly IDisbursementService _disbursementService;
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly IApprovalService _approvalService;
        private readonly IEmiService _emiService;

        public DisbursementController(
            IDisbursementService disbursementService,
            ILoanApplicationService loanApplicationService,
            IApprovalService approvalService,
            IEmiService emiService)
        {
            _disbursementService = disbursementService;
            _loanApplicationService = loanApplicationService;
            _approvalService = approvalService;
            _emiService = emiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            // Show all Approved applications that haven't been disbursed yet
            var query = new PagedQuery { Page = page, PageSize = pageSize };
            var result = await _loanApplicationService.SearchAsync(query, ApplicationStatus.Approved);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
                return RedirectToAction(nameof(Index));

            var approval = await _approvalService.GetByApplicationIdAsync(id);
            if (approval == null || approval.ApprovalStatus != ApprovalStatus.Approved)
            {
                TempData["Error"] = "Application must be approved before initiating disbursement.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Approval = approval;
            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int id, string bankName, long bankAccountNumber, DisbursmentMode disbursementMode, string notes)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var processedByUserId = int.TryParse(userIdStr, out var uid) ? uid : 3;

                var approval = await _approvalService.GetByApplicationIdAsync(id);
                if (approval == null || approval.ApprovalStatus != ApprovalStatus.Approved)
                {
                    TempData["Error"] = "Application is not approved.";
                    return RedirectToAction("Index", "Dashboard");
                }

                var application = await _loanApplicationService.GetByIdAsync(id);
                var disbursedAmount = approval.ApprovedAmount ?? application?.RequestedAmount ?? 0m;

                var randomTxnId = "TXN" + Random.Shared.Next(10000000, 99999999).ToString();
                var disbursement = new Disbursment
                {
                    ApplicationId = id,
                    AmountApproved = disbursedAmount,
                    BankAccountNumber = bankAccountNumber,
                    BankName = bankName,
                    DisbursmentMode = disbursementMode,
                    TransactionId = randomTxnId,
                    ProcessedByUserID = processedByUserId,
                    Notes = notes,
                    Status = DisbursmentStatus.Pending
                };

                // Create the pending disbursement record
                var created = await _disbursementService.CreateAsync(disbursement);

                // Process: marks Completed, generates EMI schedule, updates application status, notifies customer
                await _disbursementService.ProcessAsync(created.DisbursmentId, processedByUserId);

                TempData["Success"] = $"Disbursement of ₹{disbursedAmount:N2} processed successfully. EMI schedule generated.";
                return RedirectToAction(nameof(History));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Create), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var history = await _disbursementService.GetHistoryAsync();
            return View(history);
        }
    }
}
