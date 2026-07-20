using LOCPS.Common;
using LOCPS.Data;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Services.Implementations;

public class CreditEvaluationService : ICreditEvaluationService
{
    private readonly ICreditEvaluationRepository _repository;
    private readonly ILoanApplicationRepository _applicationRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;
    private readonly AppDbContext _db;

    public CreditEvaluationService(
        ICreditEvaluationRepository repository,
        ILoanApplicationRepository applicationRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService,
        AppDbContext db)
    {
        _repository = repository;
        _applicationRepository = applicationRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
        _db = db;
    }

    public async Task<CreditEvaluation> CalculateAndSaveAsync(int applicationId, int evaluatedByUserId)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(applicationId)
            ?? throw new ServiceException("Application not found.", 404);

        var config = await _db.ScoringConfigs.FirstOrDefaultAsync()
                     ?? new ScoringConfig
                     {
                         MinCreditScore = 650,
                         BureauScoreWeight = 35,
                         DebtToIncomeWeight = 25,
                         CreditHistoryAgeWeight = 15,
                         RepaymentConsistencyWeight = 15,
                         CreditUtilizationWeight = 10
                     };

        var dti = CalculateDTI(application);

        // Map DTI to a score on 300-900 scale
        int dtiScore = dti <= 20 ? 900 : dti <= 35 ? 800 : dti <= 45 ? 700 : dti <= 55 ? 600 : 400;

        // Base CIBIL score (pulled or generated)
        int bureauScore = 750;
        if (application.AnnualIncome > 800000) bureauScore = 800;
        else if (application.AnnualIncome < 300000) bureauScore = 620;

        int creditHistoryAgeScore = 780;
        int repaymentConsistencyScore = 820;
        int creditUtilizationScore = 740;

        // Apply weights
        double weightedScore = (bureauScore * config.BureauScoreWeight +
                               dtiScore * config.DebtToIncomeWeight +
                               creditHistoryAgeScore * config.CreditHistoryAgeWeight +
                               repaymentConsistencyScore * config.RepaymentConsistencyWeight +
                               creditUtilizationScore * config.CreditUtilizationWeight) / 100.0;

        int finalScore = Math.Clamp((int)Math.Round(weightedScore), 300, 900);

        // Recommend based on threshold set by Admin
        var recommendation = finalScore >= config.MinCreditScore ? CreditRecommendation.Approved : CreditRecommendation.Rejected;

        var evaluation = new CreditEvaluation
        {
            ApplicationId = applicationId,
            EvaluatedByUserId = evaluatedByUserId,
            EvaluatedDate = DateTime.UtcNow,
            CreditScore = finalScore,
            DebitToIncomeRatio = dti,
            PaymentHistoryScore = repaymentConsistencyScore,
            ExistingLiabilities = 0,
            CreditRecommendation = recommendation
        };

        var created = await _repository.CreateCreditEvaluationAsync(evaluation);
        await _auditLogService.LogAsync(evaluatedByUserId, Actions.Created, $"CreditEval:{created.CreditId}", null, $"Score={finalScore}, Rec={recommendation}", null, null);
        return created;
    }

    public async Task<CreditEvaluation?> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetCreditEvaluationByApplicationAsync(applicationId);

    public async Task<CreditEvaluation> ApproveAsync(int applicationId, int userId, string? comments)
    {
        var eval = await _repository.GetCreditEvaluationByApplicationAsync(applicationId)
            ?? throw new ServiceException("Credit evaluation not found.", 404);

        eval.CreditRecommendation = CreditRecommendation.Approved;
        eval.Comments = comments;
        eval.EvaluatedDate = DateTime.UtcNow;
        var updated = await _repository.UpdateCreditEvaluationAsync(eval);

        await _auditLogService.LogAsync(userId, Actions.Updated, $"CreditEval:{eval.CreditId}", "Pending", "Approved", null, null);
        await _notificationService.CreateAsync(applicationId, NotificationType.CreditEvaluated, "Credit Evaluation Approved", "Your credit evaluation has been approved.", applicationId);
        return updated;
    }

    public async Task<CreditEvaluation> RejectAsync(int applicationId, int userId, string? comments)
    {
        var eval = await _repository.GetCreditEvaluationByApplicationAsync(applicationId)
            ?? throw new ServiceException("Credit evaluation not found.", 404);

        eval.CreditRecommendation = CreditRecommendation.Rejected;
        eval.Comments = comments;
        eval.EvaluatedDate = DateTime.UtcNow;
        var updated = await _repository.UpdateCreditEvaluationAsync(eval);

        await _auditLogService.LogAsync(userId, Actions.Updated, $"CreditEval:{eval.CreditId}", "Pending", "Rejected", null, null);
        return updated;
    }

    private static decimal CalculateDTI(LoanApplication application) =>
        application.RequestedAmount > 0 ? (application.RequestedAmount / (application.AnnualIncome * 12)) * 100 : 0;
}
