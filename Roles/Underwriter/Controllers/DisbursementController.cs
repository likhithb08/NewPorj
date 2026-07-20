using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class DisbursementController : Controller
    {
        private readonly IDisbursementService _disbursementService;

        public DisbursementController(IDisbursementService disbursementService)
        {
            _disbursementService = disbursementService;
        }

        // Underwriter creates disbursements
        public IActionResult Create(int id) => View();

        // Both Admin and Underwriter can view history
        public async Task<IActionResult> History()
        {
            var history = await _disbursementService.GetHistoryAsync();
            return View(history);
        }
    }
}
