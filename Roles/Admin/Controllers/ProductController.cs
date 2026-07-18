using Microsoft.AspNetCore.Mvc;
using LOCPS.Services.Interfaces;
using LOCPS.Models;

namespace LOCPS.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILoanProductService _loanProductService;

        public ProductController(ILoanProductService loanProductService)
        {
            _loanProductService = loanProductService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _loanProductService.GetAllAsync(true);
            return View(products);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(LoanProduct product)
        {
            if (!ModelState.IsValid)
                return View(product);

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var createdByUserId = int.TryParse(userId, out var id) ? id : 1;
                await _loanProductService.CreateAsync(product, createdByUserId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Failed to create loan product.");
                return View(product);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _loanProductService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _loanProductService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LoanProduct product)
        {
            if (!ModelState.IsValid)
                return View(product);

            try
            {
                await _loanProductService.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Failed to update loan product.");
                return View(product);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _loanProductService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _loanProductService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
