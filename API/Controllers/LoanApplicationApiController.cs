using LOCPS.Common;
using LOCPS.DTOs;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanApplicationApiController : ControllerBase
{
    private readonly ILoanApplicationService _loanApplicationService;

    public LoanApplicationApiController(ILoanApplicationService loanApplicationService)
    {
        _loanApplicationService = loanApplicationService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<LoanApplicationDto>>> CreateApplication([FromBody] CreateLoanApplicationDto model)
    {
        try
        {
            var application = new LoanApplication
            {
                CustomerId = model.CustomerId,
                ProductId = model.ProductId,
                RequestedAmount = model.RequestedAmount,
                AnnualIncome = model.AnnualIncome,
                EmploymentType = model.EmploymentType,
                CreatedByUserId = 1 // TODO: Get from authenticated user
            };

            var created = await _loanApplicationService.CreateAsync(application);
            var applicationDto = new LoanApplicationDto
            {
                ApplicationId = created.ApplicationId,
                ApplicationNumber = created.ApplicationNumber,
                CustomerId = created.CustomerId,
                CustomerName = created.Customer?.FullName ?? string.Empty,
                ProductId = created.ProductId,
                ProductName = created.Product?.ProductName ?? string.Empty,
                RequestedAmount = created.RequestedAmount,
                ApprovedAmount = created.ApprovedAmount,
                AnnualIncome = created.AnnualIncome,
                EmploymentType = created.EmploymentType,
                Status = created.Status,
                CreatedAt = created.CreatedAt,
                LastUpdatedDate = created.LastUpdatedDate
            };

            return Ok(ApiResult<LoanApplicationDto>.Ok(applicationDto, "Loan application created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<LoanApplicationDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResult<LoanApplicationDto>>> GetApplication(int id)
    {
        try
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
                return NotFound(ApiResult<LoanApplicationDto>.Fail("Application not found"));

            var applicationDto = new LoanApplicationDto
            {
                ApplicationId = application.ApplicationId,
                ApplicationNumber = application.ApplicationNumber,
                CustomerId = application.CustomerId,
                CustomerName = application.Customer?.FullName ?? string.Empty,
                ProductId = application.ProductId,
                ProductName = application.Product?.ProductName ?? string.Empty,
                RequestedAmount = application.RequestedAmount,
                ApprovedAmount = application.ApprovedAmount,
                AnnualIncome = application.AnnualIncome,
                EmploymentType = application.EmploymentType,
                Status = application.Status,
                CreatedAt = application.CreatedAt,
                LastUpdatedDate = application.LastUpdatedDate
            };

            return Ok(ApiResult<LoanApplicationDto>.Ok(applicationDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<LoanApplicationDto>.Fail(ex.Message));
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResult<PagedResult<LoanApplicationDto>>>> SearchApplications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] ApplicationStatus? status = null,
        [FromQuery] int? customerId = null)
    {
        try
        {
            var query = new PagedQuery
            {
                Page = pageNumber,
                PageSize = pageSize,
                Search = searchTerm
            };

            var result = await _loanApplicationService.SearchAsync(query, status, customerId);
            var applicationDtos = result.Items.Select(a => new LoanApplicationDto
            {
                ApplicationId = a.ApplicationId,
                ApplicationNumber = a.ApplicationNumber,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer?.FullName ?? string.Empty,
                ProductId = a.ProductId,
                ProductName = a.Product?.ProductName ?? string.Empty,
                RequestedAmount = a.RequestedAmount,
                ApprovedAmount = a.ApprovedAmount,
                AnnualIncome = a.AnnualIncome,
                EmploymentType = a.EmploymentType,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                LastUpdatedDate = a.LastUpdatedDate
            }).ToList();

            var pagedResult = new PagedResult<LoanApplicationDto>
            {
                Items = applicationDtos,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return Ok(ApiResult<PagedResult<LoanApplicationDto>>.Ok(pagedResult));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<PagedResult<LoanApplicationDto>>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResult<LoanApplicationDto>>> UpdateStatus(int id, [FromBody] UpdateLoanApplicationStatusDto model)
    {
        try
        {
            var updated = await _loanApplicationService.UpdateStatusAsync(id, model.Status, 1);
            var applicationDto = new LoanApplicationDto
            {
                ApplicationId = updated.ApplicationId,
                ApplicationNumber = updated.ApplicationNumber,
                CustomerId = updated.CustomerId,
                CustomerName = updated.Customer?.FullName ?? string.Empty,
                ProductId = updated.ProductId,
                ProductName = updated.Product?.ProductName ?? string.Empty,
                RequestedAmount = updated.RequestedAmount,
                ApprovedAmount = updated.ApprovedAmount,
                AnnualIncome = updated.AnnualIncome,
                EmploymentType = updated.EmploymentType,
                Status = updated.Status,
                CreatedAt = updated.CreatedAt,
                LastUpdatedDate = updated.LastUpdatedDate
            };

            return Ok(ApiResult<LoanApplicationDto>.Ok(applicationDto, "Status updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<LoanApplicationDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResult>> DeleteApplication(int id)
    {
        try
        {
            await _loanApplicationService.DeleteAsync(id);
            return Ok(ApiResult.Ok("Loan application deleted successfully"));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResult.Fail(ex.Message));
        }
    }
}
