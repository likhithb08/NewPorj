using LOCPS.Enums;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class KycController : Controller
    {
        private readonly IKycService _kycService;
        private readonly IDocumentService _documentService;
        private readonly ILoanApplicationService _loanApplicationService;

        public KycController(IKycService kycService, IDocumentService documentService, ILoanApplicationService loanApplicationService)
        {
            _kycService = kycService;
            _documentService = documentService;
            _loanApplicationService = loanApplicationService;
        }

        [HttpGet]
        public IActionResult Verify(int id) => View();

        [HttpGet]
        public IActionResult History(int id) => View();

        [HttpPost]
        public async Task<IActionResult> Initiate(int applicationId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var officerId = int.TryParse(userId, out var id) ? id : 0;
                
                await _kycService.InitiateKycAsync(applicationId, officerId);
                TempData["Success"] = "KYC Initiated. Customer has been notified to upload documents.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Loan", new { id = applicationId });
        }

        [HttpPost]
        public async Task<IActionResult> Verify(int kycId, int applicationId, KycStatus status, string remarks)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var officerId = int.TryParse(userId, out var id) ? id : 0;

                if (status == KycStatus.Verified)
                {
                    await _kycService.VerifyAsync(kycId, officerId, remarks ?? string.Empty);
                    TempData["Success"] = "KYC verified successfully.";
                }
                else if (status == KycStatus.Rejected)
                {
                    await _kycService.RejectAsync(kycId, officerId, remarks ?? string.Empty);
                    TempData["Success"] = "KYC rejected. Customer notified.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Loan", new { id = applicationId });
        }
    }
}
