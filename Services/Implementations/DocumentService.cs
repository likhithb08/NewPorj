using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public DocumentService(
        IDocumentRepository repository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<Document> UploadAsync(Document document)
    {
        document.UploadDate = DateTime.UtcNow;
        document.DocumentStatus = DocumentStatus.Pending;
        var created = await _repository.CreateAsync(document)
            ?? throw new ServiceException("Failed to upload document.");

        await _auditLogService.LogAsync(document.UploadedByUserId, Actions.Created, $"Document:{created.DocumentId}", null, "Uploaded", null, null);
        return created;
    }

    public async Task<IEnumerable<Document>> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetByApplicationIdAsync(applicationId);

    public async Task<Document> ApproveAsync(int documentId, int verifierUserId, string remarks)
    {
        var doc = await _repository.GetByIdWithDetailsAsync(documentId)
            ?? throw new ServiceException("Document not found.", 404);

        doc.DocumentStatus = DocumentStatus.Verified;
        doc.VerifiedByUserId = verifierUserId;
        doc.Remarks = remarks;

        var updated = await _repository.UpdateAsync(doc);
        await _auditLogService.LogAsync(verifierUserId, Actions.Updated, $"Document:{documentId}", "Pending", "Verified", null, null);
        return updated;
    }

    public async Task<Document> RejectAsync(int documentId, int verifierUserId, string remarks)
    {
        var doc = await _repository.GetByIdWithDetailsAsync(documentId)
            ?? throw new ServiceException("Document not found.", 404);

        doc.DocumentStatus = DocumentStatus.Rejected;
        doc.VerifiedByUserId = verifierUserId;
        doc.Remarks = remarks;

        var updated = await _repository.UpdateAsync(doc);
        await _auditLogService.LogAsync(verifierUserId, Actions.Updated, $"Document:{documentId}", "Pending", "Rejected", null, null);
        await _notificationService.CreateAsync(doc.Application.CustomerId, NotificationType.DocumentRequest, "Document Rejected", $"Your document {doc.FileName} has been rejected. Remarks: {remarks}", doc.ApplicationId);
        return updated;
    }
}
