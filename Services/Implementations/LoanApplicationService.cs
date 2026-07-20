using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class LoanApplicationService : ILoanApplicationService
{
    private readonly ILoanApplicationRepository _repository;
    private readonly ILoanProductRepository _productRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public LoanApplicationService(
        ILoanApplicationRepository repository,
        ILoanProductRepository productRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _repository = repository;
        _productRepository = productRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<LoanApplication> CreateAsync(LoanApplication application)
    {
        var product = await _productRepository.GetLoanProductByIdAsync(application.ProductId)
            ?? throw new ServiceException("Invalid loan product.", 400);

        if (application.RequestedAmount < product.MinAmount || application.RequestedAmount > product.MaxAmount)
            throw new ServiceException($"Amount must be between {product.MinAmount} and {product.MaxAmount}.");

        application.ApplicationNumber = GenerateApplicationNumber();
        application.Status = ApplicationStatus.Submitted;
        application.CreatedAt = DateTime.UtcNow;

        var created = await _repository.CreateLoanApplicationAsync(application)
            ?? throw new ServiceException("Failed to create application.");

        await _auditLogService.LogAsync(application.CreatedByUserId, Actions.Created, $"Application:{created.ApplicationId}", null, created.ApplicationNumber, null, null);
        await _notificationService.CreateAsync(application.CustomerId, NotificationType.ApplicationSubmitted, "Loan Application Submitted", $"Application {created.ApplicationNumber} submitted.", created.ApplicationId);
        return created;
    }

    public async Task<LoanApplication?> GetByIdAsync(int id) => await _repository.GetWithDetailsAsync(id);

    public Task<PagedResult<LoanApplication>> SearchAsync(PagedQuery query, ApplicationStatus? status = null, int? customerId = null) =>
        _repository.SearchAsync(query, status, customerId);

    public async Task<LoanApplication> UpdateAsync(LoanApplication application)
    {
        application.LastUpdatedDate = DateTime.UtcNow;
        return await _repository.UpdateLoanApplicationAsync(application)
            ?? throw new ServiceException("Application not found.", 404);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var app = await _repository.GetLoanApplicationByIdAsync(id)
            ?? throw new ServiceException("Application not found.", 404);
        await _repository.DeleteLoanApplicationAsync(app);
        return true;
    }

    public async Task<LoanApplication> UpdateStatusAsync(int applicationId, ApplicationStatus status, int actorUserId)
    {
        var app = await _repository.GetLoanApplicationByIdAsync(applicationId)
            ?? throw new ServiceException("Application not found.", 404);

        var old = app.Status.ToString();
        app.Status = status;
        app.LastUpdatedDate = DateTime.UtcNow;
        var updated = await _repository.UpdateLoanApplicationAsync(app)
            ?? throw new ServiceException("Update failed.");

        await _auditLogService.LogAsync(actorUserId, Actions.Updated, $"Application:{applicationId}", old, status.ToString(), null, null);

        if (status == ApplicationStatus.UnderReview)
            await _notificationService.CreateAsync(app.CustomerId, NotificationType.ApplicationStatusUpdate, "Application Forwarded", $"Your application {app.ApplicationNumber} has been successfully verified and forwarded to underwriting.", applicationId);
        else if (status == ApplicationStatus.KYCVerified)
            await _notificationService.CreateAsync(app.CustomerId, NotificationType.ApplicationStatusUpdate, "Verification Complete", $"Your application {app.ApplicationNumber} verification is complete.", applicationId);

        return updated;
    }

    public string GenerateApplicationNumber() => $"LOC-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
}
