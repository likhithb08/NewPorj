using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var activeRole = Request.Cookies["locps_demo_role"] ?? "officer";
            if (activeRole == "officer" || activeRole == "admin")
            {
                return View();
            }

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
            var productList = products.ToList();
            ViewBag.Products = new SelectList(productList, "ProductId", "ProductName");

            // Pass product details as JSON for the client-side EMI calculator
            var productDetails = productList.Select(p => new
            {
                productId      = p.ProductId,
                productName    = p.ProductName,
                minAmount      = p.MinAmount,
                maxAmount      = p.MaxAmount,
                interestRate   = p.InterestRate,
                maxTenureMonths = p.MaxTenureMonths,
                processingFee  = p.ProcessingFee,
                description    = p.ProductDescription
            });
            ViewBag.ProductDetailsJson = System.Text.Json.JsonSerializer.Serialize(productDetails);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LoanApplication application)
        {
            // Clear validation for properties that are not part of the form
            ModelState.Remove(nameof(application.ApplicationNumber));
            ModelState.Remove(nameof(application.Customer));
            ModelState.Remove(nameof(application.Product));
            ModelState.Remove(nameof(application.CreatedBy));

            if (!ModelState.IsValid)
            {
                var products = await _loanProductService.GetAllAsync(true);
                var productList = products.ToList();
                ViewBag.Products = new SelectList(productList, "ProductId", "ProductName");
                var productDetails = productList.Select(p => new
                {
                    productId      = p.ProductId,
                    productName    = p.ProductName,
                    minAmount      = p.MinAmount,
                    maxAmount      = p.MaxAmount,
                    interestRate   = p.InterestRate,
                    maxTenureMonths = p.MaxTenureMonths,
                    processingFee  = p.ProcessingFee,
                    description    = p.ProductDescription
                });
                ViewBag.ProductDetailsJson = System.Text.Json.JsonSerializer.Serialize(productDetails);
                return View(application);
            }

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                application.CustomerId = int.TryParse(userId, out var id) ? id : 0;
                application.CreatedByUserId = application.CustomerId;
                await _loanApplicationService.CreateAsync(application);
                TempData["Success"] = "Loan application submitted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var products = await _loanProductService.GetAllAsync(true);
                var productList = products.ToList();
                ViewBag.Products = new SelectList(productList, "ProductId", "ProductName");
                var productDetails = productList.Select(p => new
                {
                    productId      = p.ProductId,
                    productName    = p.ProductName,
                    minAmount      = p.MinAmount,
                    maxAmount      = p.MaxAmount,
                    interestRate   = p.InterestRate,
                    maxTenureMonths = p.MaxTenureMonths,
                    processingFee  = p.ProcessingFee,
                    description    = p.ProductDescription
                });
                ViewBag.ProductDetailsJson = System.Text.Json.JsonSerializer.Serialize(productDetails);
                return View(application);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var activeRole = Request.Cookies["locps_demo_role"] ?? "officer";
            if (activeRole == "officer" || activeRole == "admin")
            {
                return View();
            }

            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
                return NotFound();

            var kyc = await _kycService.GetByApplicationIdAsync(id);
            ViewBag.Kyc = kyc;

            return View(application);
        }

        [HttpGet]
        public IActionResult Edit(int id) => View();

        [HttpPost]
        public async Task<IActionResult> SubmitKyc(Kyc kyc)
        {
            try
            {
                await _kycService.SubmitAsync(kyc);
                TempData["Success"] = "KYC details submitted successfully.";
                return RedirectToAction(nameof(Details), new { id = kyc.ApplicationId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = kyc.ApplicationId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(int applicationId, IFormFile file, DocumentType documentType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "Please select a file to upload.";
                    return RedirectToAction(nameof(Details), new { id = applicationId });
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var customerId = int.TryParse(userId, out var id) ? id : 0;

                // Save file logic (mocked saving to wwwroot/uploads for now)
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
                
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var doc = new Document
                {
                    ApplicationId = applicationId,
                    DocumentType = documentType,
                    FileName = file.FileName,
                    FilePath = "uploads/" + fileName,
                    FileSize = file.Length,
                    UploadedByUserId = customerId
                };

                // Assuming IDocumentService is injected, wait we didn't inject it in CustomerController.
                // Let's resolve it from HttpContext.RequestServices to avoid constructor changes if possible,
                // or just modify constructor. I will resolve it.
                var documentService = HttpContext.RequestServices.GetRequiredService<IDocumentService>();
                await documentService.UploadAsync(doc);
                
                TempData["Success"] = "Document uploaded successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id = applicationId });
        }
    }
}
