using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface ICreditEvaluationService
{
    Task<CreditEvaluation> CalculateAndSaveAsync(int applicationId, int evaluatedByUserId);
    Task<CreditEvaluation?> GetByApplicationIdAsync(int applicationId);
    Task<CreditEvaluation> ApproveAsync(int applicationId, int userId, string? comments);
    Task<CreditEvaluation> RejectAsync(int applicationId, int userId, string? comments);
}
