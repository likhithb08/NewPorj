using LOCPS.DTOs;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanProductApiController : ControllerBase
{
    private readonly ILoanProductService _loanProductService;

    public LoanProductApiController(ILoanProductService loanProductService)
    {
        _loanProductService = loanProductService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<LoanProductDto>>> CreateProduct([FromBody] CreateLoanProductDto model)
    {
        try
        {
            var product = new LoanProduct
            {
                ProductName = model.ProductName,
                ProductDescription = model.ProductDescription,
                MinAmount = model.MinAmount,
                MaxAmount = model.MaxAmount,
                InterestRate = model.InterestRate,
                MaxTenureMonths = model.MaxTenureMonths,
                ProcessingFee = model.ProcessingFee,
                CreatedByUserId = 1 // TODO: Get from authenticated user
            };

            var created = await _loanProductService.CreateAsync(product, 1);
            var productDto = new LoanProductDto
            {
                ProductId = created.ProductId,
                ProductName = created.ProductName,
                ProductDescription = created.ProductDescription,
                MinAmount = created.MinAmount,
                MaxAmount = created.MaxAmount,
                InterestRate = created.InterestRate,
                MaxTenureMonths = created.MaxTenureMonths,
                ProcessingFee = created.ProcessingFee,
                IsActive = created.IsActive,
                CreatedAt = created.CreatedAt
            };

            return Ok(ApiResult<LoanProductDto>.Ok(productDto, "Loan product created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<LoanProductDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResult<LoanProductDto>>> GetProduct(int id)
    {
        try
        {
            var product = await _loanProductService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResult<LoanProductDto>.Fail("Product not found"));

            var productDto = new LoanProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                MinAmount = product.MinAmount,
                MaxAmount = product.MaxAmount,
                InterestRate = product.InterestRate,
                MaxTenureMonths = product.MaxTenureMonths,
                ProcessingFee = product.ProcessingFee,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };

            return Ok(ApiResult<LoanProductDto>.Ok(productDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<LoanProductDto>.Fail(ex.Message));
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<List<LoanProductDto>>>> GetAllProducts([FromQuery] bool activeOnly = true)
    {
        try
        {
            var products = await _loanProductService.GetAllAsync(activeOnly);
            var productDtos = products.Select(p => new LoanProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                MinAmount = p.MinAmount,
                MaxAmount = p.MaxAmount,
                InterestRate = p.InterestRate,
                MaxTenureMonths = p.MaxTenureMonths,
                ProcessingFee = p.ProcessingFee,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(ApiResult<List<LoanProductDto>>.Ok(productDtos));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<List<LoanProductDto>>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResult<LoanProductDto>>> UpdateProduct(int id, [FromBody] UpdateLoanProductDto model)
    {
        try
        {
            var product = new LoanProduct
            {
                ProductId = id,
                ProductName = model.ProductName,
                ProductDescription = model.ProductDescription,
                MinAmount = model.MinAmount,
                MaxAmount = model.MaxAmount,
                InterestRate = model.InterestRate,
                MaxTenureMonths = model.MaxTenureMonths,
                ProcessingFee = model.ProcessingFee,
                IsActive = model.IsActive
            };

            var updated = await _loanProductService.UpdateAsync(product);
            var productDto = new LoanProductDto
            {
                ProductId = updated.ProductId,
                ProductName = updated.ProductName,
                ProductDescription = updated.ProductDescription,
                MinAmount = updated.MinAmount,
                MaxAmount = updated.MaxAmount,
                InterestRate = updated.InterestRate,
                MaxTenureMonths = updated.MaxTenureMonths,
                ProcessingFee = updated.ProcessingFee,
                IsActive = updated.IsActive,
                CreatedAt = updated.CreatedAt
            };

            return Ok(ApiResult<LoanProductDto>.Ok(productDto, "Loan product updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<LoanProductDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResult>> DeleteProduct(int id)
    {
        try
        {
            await _loanProductService.DeleteAsync(id);
            return Ok(ApiResult.Ok("Loan product deleted successfully"));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResult.Fail(ex.Message));
        }
    }
}
