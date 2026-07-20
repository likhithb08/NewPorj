using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class DisbursementService : IDisbursementService
{
    private readonly IDisbursmentRepository _repository;
    private readonly ILoanApplicationRepository _applicationRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public DisbursementService(
        IDisbursmentRepository repository,
        ILoanApplicationRepository applicationRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _repository = repository;
        _applicationRepository = applicationRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<Disbursment> CreateAsync(Disbursment disbursement)
    {
        disbursement.DisbursmentDate = DateTime.UtcNow;
        disbursement.Status = DisbursmentStatus.Pending;
        var created = await _repository.CreateDisbursmentAsync(disbursement)
            ?? throw new ServiceException("Failed to create disbursement.");
        await _auditLogService.LogAsync(disbursement.ProcessedByUserID, Actions.Created, $"Disbursement:{created.DisbursmentId}", null, "Created", null, null);
        return created;
    }

    public async Task<Disbursment?> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetDisbursmentByApplicationIdAsync(applicationId);

    public async Task<IEnumerable<Disbursment>> GetHistoryAsync() =>
        await _repository.GetPendingDisbursmentsAsync(DisbursmentStatus.Completed);

    public async Task<Disbursment> ProcessAsync(int disbursementId, int processedByUserId)
    {
        var disbursement = await _repository.GetByIdAsync(disbursementId)
            ?? throw new ServiceException("Disbursement not found.", 404);

        disbursement.Status = DisbursmentStatus.Completed;
        disbursement.ProcessedByUserID = processedByUserId;
        disbursement.DisbursmentDate = DateTime.UtcNow;
        var updated = await _repository.UpdateDisbursmentAsync(disbursement)
            ?? throw new ServiceException("Failed to process disbursement.");

        var application = await _applicationRepository.GetWithDetailsAsync(disbursement.ApplicationId);
        if (application != null)
        {
            application.Status = ApplicationStatus.Disbursed;
            await _applicationRepository.UpdateLoanApplicationAsync(application);
            await _notificationService.CreateAsync(application.CustomerId, NotificationType.DisbursementProcessed, "Disbursement Processed", $"Your loan disbursement of {disbursement.AmountApproved} has been processed.", application.ApplicationId);
        }

        await _auditLogService.LogAsync(processedByUserId, Actions.Updated, $"Disbursement:{disbursementId}", "Pending", "Completed", null, null);
        return updated;
    }
}
