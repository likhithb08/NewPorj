using LOCPS.Enums;
using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces
{
    public interface ICreditEvaluationRepository : IGenericRepository<CreditEvaluation>
    {
        Task<CreditEvaluation> CreateCreditEvaluationAsync(CreditEvaluation CreditEvaluation);
        Task<CreditEvaluation?> GetCreditEvaluationByApplicationAsync(int ApplicationId);   
        Task<CreditEvaluation> UpdateCreditEvaluationAsync(CreditEvaluation CreditEvaluation);
        Task<IEnumerable<CreditEvaluation>> GetPendingCreditEvaluationAsync(CreditRecommendation CreditRecommendations);
        Task<CreditEvaluation?> GetEvaluationByofficerAsync(int EvaluatedByUserId);
    }
}
