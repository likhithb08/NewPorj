using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class KycService : IKycService
{
    private readonly IKycRepository _repository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;
    private readonly ILoanApplicationRepository _applicationRepository;

    public KycService(
        IKycRepository repository,
        IAuditLogService auditLogService,
        INotificationService notificationService,
        ILoanApplicationRepository applicationRepository)
    {
        _repository = repository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
        _applicationRepository = applicationRepository;
    }

    public async Task<Kyc> SubmitAsync(Kyc kyc)
    {
        kyc.VerificationStatus = KycStatus.Pending;
        kyc.CreatedDate = DateTime.UtcNow;
        kyc.IsActive = true;
        var created = await _repository.CreateKycAsync(kyc)
            ?? throw new ServiceException("Failed to submit KYC.");
        await _auditLogService.LogAsync(kyc.ApplicationId, Actions.Created, $"KYC:{created.KycId}", null, "Submitted", null, null);
        return created;
    }

    public async Task<Kyc?> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetKycByApplicationIdAsync(applicationId);

    public async Task<Kyc> VerifyAsync(int kycId, int verifiedByUserId, string remarks)
    {
        var kyc = await _repository.GetByIdAsync(kycId)
            ?? throw new ServiceException("KYC not found.", 404);

        kyc.VerificationStatus = KycStatus.Verified;
        kyc.VerifiedByUserId = verifiedByUserId;
        kyc.VerifiedDate = DateTime.UtcNow;
        kyc.Remarks = remarks;
        var updated = await _repository.UpdateKycAsync(kyc)
            ?? throw new ServiceException("Failed to verify KYC.");

        await _auditLogService.LogAsync(verifiedByUserId, Actions.Updated, $"KYC:{kycId}", "Pending", "Verified", null, null);
        await _notificationService.CreateAsync(kyc.ApplicationId, NotificationType.KYCVerified, "KYC Verified", "Your KYC has been verified successfully.", kyc.ApplicationId);

        var app = await _applicationRepository.GetWithDetailsAsync(kyc.ApplicationId);
        if (app != null)
        {
            app.Status = ApplicationStatus.KYCVerified;
            await _applicationRepository.UpdateLoanApplicationAsync(app);
        }

        return updated;
    }

    public async Task<Kyc> RejectAsync(int kycId, int verifiedByUserId, string remarks)
    {
        var kyc = await _repository.GetByIdAsync(kycId)
            ?? throw new ServiceException("KYC not found.", 404);

        kyc.VerificationStatus = KycStatus.Rejected;
        kyc.VerifiedByUserId = verifiedByUserId;
        kyc.VerifiedDate = DateTime.UtcNow;
        kyc.Remarks = remarks;
        var updated = await _repository.UpdateKycAsync(kyc)
            ?? throw new ServiceException("Failed to reject KYC.");

        await _auditLogService.LogAsync(verifiedByUserId, Actions.Updated, $"KYC:{kycId}", "Pending", "Rejected", null, null);
        await _notificationService.CreateAsync(kyc.ApplicationId, NotificationType.ApplicationStatusUpdate, "KYC Rejected", $"Your KYC has been rejected. Remarks: {remarks}", kyc.ApplicationId);

        return updated;
    }

    public async Task InitiateKycAsync(int applicationId, int loanOfficerId)
    {
        var app = await _applicationRepository.GetLoanApplicationByIdAsync(applicationId)
            ?? throw new ServiceException("Application not found.");

        app.Status = ApplicationStatus.KYCPending;
        await _applicationRepository.UpdateLoanApplicationAsync(app);

        await _auditLogService.LogAsync(loanOfficerId, Actions.Updated, $"Application:{applicationId}", "Submitted", "KYCPending", null, null);
        await _notificationService.CreateAsync(app.CustomerId, NotificationType.DocumentRequest, "Action Required: Upload Documents", $"Your loan application {app.ApplicationNumber} requires KYC documents. Please log in and upload them.", applicationId);
    }
}
