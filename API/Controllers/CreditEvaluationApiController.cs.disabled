using LOCPS.DTOs;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditEvaluationApiController : ControllerBase
{
    private readonly ICreditEvaluationService _creditEvaluationService;

    public CreditEvaluationApiController(ICreditEvaluationService creditEvaluationService)
    {
        _creditEvaluationService = creditEvaluationService;
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<ApiResult<CreditEvaluation>>> CalculateCreditEvaluation([FromBody] CreditEvaluationRequestDto request)
    {
        try
        {
            var evaluation = await _creditEvaluationService.CalculateAndSaveAsync(request.ApplicationId, request.EvaluatedByUserId);
            return Ok(ApiResult<CreditEvaluation>.Ok(evaluation, "Credit evaluation calculated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<CreditEvaluation>.Fail(ex.Message));
        }
    }

    [HttpGet("application/{applicationId}")]
    public async Task<ActionResult<ApiResult<CreditEvaluation>>> GetByApplicationId(int applicationId)
    {
        try
        {
            var evaluation = await _creditEvaluationService.GetByApplicationIdAsync(applicationId);
            if (evaluation == null)
                return NotFound(ApiResult<CreditEvaluation>.Fail("Credit evaluation not found"));

            return Ok(ApiResult<CreditEvaluation>.Ok(evaluation));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<CreditEvaluation>.Fail(ex.Message));
        }
    }

    [HttpPut("approve")]
    public async Task<ActionResult<ApiResult<CreditEvaluation>>> ApproveCreditEvaluation([FromBody] CreditEvaluationActionDto request)
    {
        try
        {
            var approved = await _creditEvaluationService.ApproveAsync(request.ApplicationId, request.UserId, request.Comments);
            return Ok(ApiResult<CreditEvaluation>.Ok(approved, "Credit evaluation approved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<CreditEvaluation>.Fail(ex.Message));
        }
    }

    [HttpPut("reject")]
    public async Task<ActionResult<ApiResult<CreditEvaluation>>> RejectCreditEvaluation([FromBody] CreditEvaluationActionDto request)
    {
        try
        {
            var rejected = await _creditEvaluationService.RejectAsync(request.ApplicationId, request.UserId, request.Comments);
            return Ok(ApiResult<CreditEvaluation>.Ok(rejected, "Credit evaluation rejected successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<CreditEvaluation>.Fail(ex.Message));
        }
    }
}

public class CreditEvaluationRequestDto
{
    public int ApplicationId { get; set; }
    public int EvaluatedByUserId { get; set; }
}

public class CreditEvaluationActionDto
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public string? Comments { get; set; }
}
