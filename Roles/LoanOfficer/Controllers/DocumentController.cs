using LOCPS.Enums;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public IActionResult Validate(int id) => View();

        [HttpGet]
        public IActionResult Upload(int id) => View();

        [HttpPost]
        public async Task<IActionResult> Verify(int documentId, int applicationId, DocumentStatus status, string remarks)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var officerId = int.TryParse(userId, out var id) ? id : 0;

                if (status == DocumentStatus.Verified)
                {
                    await _documentService.ApproveAsync(documentId, officerId, remarks ?? string.Empty);
                    TempData["Success"] = "Document verified successfully.";
                }
                else if (status == DocumentStatus.Rejected)
                {
                    await _documentService.RejectAsync(documentId, officerId, remarks ?? string.Empty);
                    TempData["Success"] = "Document rejected. Customer notified.";
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
