using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class CreditController : Controller
    {
        private readonly ICreditEvaluationService _creditService;

        public CreditController(ICreditEvaluationService creditService)
        {
            _creditService = creditService;
        }

        [HttpGet]
        public IActionResult Evaluate() => View();

        [HttpGet]
        public IActionResult Details(int id) => View();

        [HttpPost]
        public async Task<IActionResult> Evaluate(int applicationId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var officerId = int.TryParse(userId, out var id) ? id : 0;

                await _creditService.CalculateAndSaveAsync(applicationId, officerId);
                TempData["Success"] = "Credit evaluation completed successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Loan", new { id = applicationId });
        }
    }
}
