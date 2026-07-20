using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class ApprovalService : IApprovalService
{
    private readonly IApprovalRepository _repository;
    private readonly ILoanApplicationRepository _applicationRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public ApprovalService(
        IApprovalRepository repository,
        ILoanApplicationRepository applicationRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _repository = repository;
        _applicationRepository = applicationRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<Approval> ApproveLoanAsync(int applicationId, int approverUserId, decimal approvedAmount, int tenureMonths, decimal interestRate, string? comments)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(applicationId)
            ?? throw new ServiceException("Application not found.", 404);

        var approval = new Approval
        {
            ApplicationId = applicationId,
            ApprovedByUserId = approverUserId,
            ApprovedAmount = approvedAmount,
            ApprovedInterestRate = interestRate,
            ApprovedTenureMonths = tenureMonths,
            ApprovalStatus = ApprovalStatus.Approved,
            ApprovalDate = DateTime.UtcNow,
            Comments = comments ?? string.Empty
        };

        var created = await _repository.CreateApprovalAsync(approval)
            ?? throw new ServiceException("Failed to approve loan.");

        application.Status = ApplicationStatus.Approved;
        application.ApprovedAmount = approvedAmount;
        await _applicationRepository.UpdateLoanApplicationAsync(application);

        await _auditLogService.LogAsync(approverUserId, Actions.Updated, $"Approval:{created.ApprovalId}", null, "Approved", null, null);
        await _notificationService.CreateAsync(application.CustomerId, NotificationType.ApprovalUpdate, "Loan Approved", $"Your loan of {approvedAmount} has been approved.", applicationId);
        return created;
    }

    public async Task<Approval> RejectLoanAsync(int applicationId, int approverUserId, string reason, string? comments)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(applicationId)
            ?? throw new ServiceException("Application not found.", 404);

        var approval = new Approval
        {
            ApplicationId = applicationId,
            ApprovedByUserId = approverUserId,
            ApprovalStatus = ApprovalStatus.Rejected,
            ApprovalDate = DateTime.UtcNow,
            Comments = comments ?? string.Empty,
            RejectionReason = reason
        };

        var created = await _repository.CreateApprovalAsync(approval)
            ?? throw new ServiceException("Failed to reject loan.");

        application.Status = ApplicationStatus.Rejected;
        await _applicationRepository.UpdateLoanApplicationAsync(application);

        await _auditLogService.LogAsync(approverUserId, Actions.Updated, $"Approval:{created.ApprovalId}", null, "Rejected", null, null);
        await _notificationService.CreateAsync(application.CustomerId, NotificationType.ApprovalUpdate, "Loan Rejected", $"Your loan application has been rejected. Reason: {reason}", applicationId);
        return created;
    }

    public async Task<Approval?> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetApprovalByApplicationIdAsync(applicationId);

    public async Task<IEnumerable<Approval>> GetHistoryAsync() =>
        await _repository.GetAllAsync();
}
