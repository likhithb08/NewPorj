using Microsoft.AspNetCore.Mvc;
using LOCPS.Services.Interfaces;
using LOCPS.Models;
using LOCPS.Enums;
using LOCPS.Common;

namespace LOCPS.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly ILoanProductService _loanProductService;
        private readonly IKycService _kycService;

        public CustomerController(ILoanApplicationService loanApplicationService, ILoanProductService loanProductService, IKycService kycService)
        {
            _loanApplicationService = loanApplicationService;
            _loanProductService = loanProductService;
            _kycService = kycService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var customerId = int.TryParse(userId, out var id) ? id : 0;
            
            if (customerId == 0)
                return RedirectToAction("Login", "Account");

            var query = new PagedQuery { Page = 1, PageSize = 10 };
            var result = await _loanApplicationService.SearchAsync(query, null, customerId);
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
                application.CustomerId = int.TryParse(userId, out var id) ? id : 0;
                application.CreatedByUserId = application.CustomerId;
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

            var kyc = await _kycService.GetByApplicationIdAsync(id);
            ViewBag.Kyc = kyc;

            return View(application);
        }

        public async Task<IActionResult> SubmitKyc(int applicationId, Kyc kyc)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details), new { id = applicationId });

            try
            {
                kyc.ApplicationId = applicationId;
                await _kycService.SubmitAsync(kyc);
                return RedirectToAction(nameof(Details), new { id = applicationId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Details), new { id = applicationId });
            }
        }
    }
}
