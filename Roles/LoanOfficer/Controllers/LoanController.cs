using Microsoft.AspNetCore.Mvc;
using LOCPS.Services.Interfaces;
using LOCPS.Models;
using LOCPS.Enums;
using LOCPS.Common;

namespace LOCPS.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly IKycService _kycService;
        private readonly IDocumentService _documentService;
        private readonly ICreditEvaluationService _creditService;

        public LoanController(
            ILoanApplicationService loanApplicationService,
            IKycService kycService,
            IDocumentService documentService,
            ICreditEvaluationService creditService)
        {
            _loanApplicationService = loanApplicationService;
            _kycService = kycService;
            _documentService = documentService;
            _creditService = creditService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, ApplicationStatus? status = null)
        {
            // Loan Officer sees: Submitted, KycPending, KycVerified
            // But let's allow them to search all or specific statuses
            var query = new PagedQuery { Page = page, PageSize = pageSize };
            var result = await _loanApplicationService.SearchAsync(query, status);
            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
                return NotFound();

            var kyc = await _kycService.GetByApplicationIdAsync(id);
            var documents = await _documentService.GetByApplicationIdAsync(id);
            var creditEval = await _creditService.GetByApplicationIdAsync(id);

            ViewBag.Kyc = kyc;
            ViewBag.Documents = documents;
            ViewBag.CreditEvaluation = creditEval;

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> ForwardToUnderwriter(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var actorUserId = int.TryParse(userId, out var uid) ? uid : 0;

                // Validate before forwarding
                var kyc = await _kycService.GetByApplicationIdAsync(id);
                if (kyc == null || kyc.VerificationStatus != KycStatus.Verified)
                {
                    TempData["Error"] = "Cannot forward: KYC is not verified.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var creditEval = await _creditService.GetByApplicationIdAsync(id);
                if (creditEval == null)
                {
                    TempData["Error"] = "Cannot forward: Credit score is not evaluated.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await _loanApplicationService.UpdateStatusAsync(id, ApplicationStatus.UnderReview, actorUserId);
                TempData["Success"] = "Application successfully forwarded to Underwriter.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
