using LOCPS.DTOs;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovalApiController : ControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalApiController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpPost("approve")]
    public async Task<ActionResult<ApiResult<Approval>>> ApproveLoan([FromBody] ApproveLoanDto request)
    {
        try
        {
            var approval = await _approvalService.ApproveLoanAsync(
                request.ApplicationId,
                request.ApproverUserId,
                request.ApprovedAmount,
                request.TenureMonths,
                request.InterestRate,
                request.Comments);
            return Ok(ApiResult<Approval>.Ok(approval, "Loan approved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<Approval>.Fail(ex.Message));
        }
    }

    [HttpPost("reject")]
    public async Task<ActionResult<ApiResult<Approval>>> RejectLoan([FromBody] RejectLoanDto request)
    {
        try
        {
            var approval = await _approvalService.RejectLoanAsync(
                request.ApplicationId,
                request.ApproverUserId,
                request.Reason,
                request.Comments);
            return Ok(ApiResult<Approval>.Ok(approval, "Loan rejected successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<Approval>.Fail(ex.Message));
        }
    }

    [HttpGet("application/{applicationId}")]
    public async Task<ActionResult<ApiResult<Approval>>> GetByApplicationId(int applicationId)
    {
        try
        {
            var approval = await _approvalService.GetByApplicationIdAsync(applicationId);
            if (approval == null)
                return NotFound(ApiResult<Approval>.Fail("Approval not found"));

            return Ok(ApiResult<Approval>.Ok(approval));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<Approval>.Fail(ex.Message));
        }
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResult<List<Approval>>>> GetHistory()
    {
        try
        {
            var history = await _approvalService.GetHistoryAsync();
            return Ok(ApiResult<List<Approval>>.Ok(history.ToList()));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<List<Approval>>.Fail(ex.Message));
        }
    }
}

public class ApproveLoanDto
{
    public int ApplicationId { get; set; }
    public int ApproverUserId { get; set; }
    public decimal ApprovedAmount { get; set; }
    public int TenureMonths { get; set; }
    public decimal InterestRate { get; set; }
    public string? Comments { get; set; }
}

public class RejectLoanDto
{
    public int ApplicationId { get; set; }
    public int ApproverUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Comments { get; set; }
}
