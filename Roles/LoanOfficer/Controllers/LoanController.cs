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
        private readonly ILoanProductService _loanProductService;

        public LoanController(ILoanApplicationService loanApplicationService, ILoanProductService loanProductService)
        {
            _loanApplicationService = loanApplicationService;
            _loanProductService = loanProductService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, ApplicationStatus? status = null)
        {
            var query = new PagedQuery { Page = page, PageSize = pageSize };
            var result = await _loanApplicationService.SearchAsync(query, status);
            return View(result);
        }

        public async Task<IActionResult> Create()
        {
            var products = await _loanProductService.GetAllAsync(true);
            ViewBag.Products = products;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LoanApplication application)
        {
            if (!ModelState.IsValid)
            {
                var products = await _loanProductService.GetAllAsync(true);
                ViewBag.Products = products;
                return View(application);
            }

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                application.CreatedByUserId = int.TryParse(userId, out var id) ? id : 1;
                await _loanApplicationService.CreateAsync(application);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var products = await _loanProductService.GetAllAsync(true);
                ViewBag.Products = products;
                return View(application);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
                return NotFound();

            return View(application);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
                return NotFound();

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LoanApplication application)
        {
            if (!ModelState.IsValid)
                return View(application);

            try
            {
                await _loanApplicationService.UpdateAsync(application);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(application);
            }
        }

        public async Task<IActionResult> UpdateStatus(int id, ApplicationStatus status)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var actorUserId = int.TryParse(userId, out var idVal) ? idVal : 1;
            await _loanApplicationService.UpdateStatusAsync(id, status, actorUserId);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
