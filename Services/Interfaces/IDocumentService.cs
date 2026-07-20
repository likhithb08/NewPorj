using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface IDocumentService
{
    Task<Document> UploadAsync(Document document);
    Task<IEnumerable<Document>> GetByApplicationIdAsync(int applicationId);
    Task<Document> ApproveAsync(int documentId, int verifierUserId, string remarks);
    Task<Document> RejectAsync(int documentId, int verifierUserId, string remarks);
}
