using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces;

public interface IAuditLogRepository : IGenericRepository<Auditlog>
{
    Task<Auditlog> CreateAsync(Auditlog log);
    Task<PagedResult<Auditlog>> GetPagedAsync(PagedQuery query);
    Task<IEnumerable<Auditlog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Auditlog>> GetAuditLogsForApplicationAsync(int applicationId);
}

public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<Document> CreateAsync(Document document);
    Task<Document?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Document>> GetByApplicationIdAsync(int applicationId);
    new Task<Document> UpdateAsync(Document document);
}
