using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces;

public interface IAuditLogRepository : IGenericRepository<Auditlog>
{
    Task<Auditlog> CreateAsync(Auditlog log);
    Task<PagedResult<Auditlog>> GetPagedAsync(PagedQuery query);
    Task<IEnumerable<Auditlog>> GetByUserIdAsync(int userId);
}

public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<Document> CreateAsync(Document document);
    Task<Document?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Document>> GetByApplicationIdAsync(int applicationId);
    Task<Document> UpdateAsync(Document document);
}

public interface IEmiRepository : IGenericRepository<Emi>
{
    Task<Emi> CreateAsync(Emi emi);
    Task<IEnumerable<Emi>> GetByApplicationIdAsync(int applicationId);
    Task<Emi?> GetByIdAsync(int id);
    Task<Emi> UpdateAsync(Emi emi);
    Task CreateScheduleAsync(int applicationId, decimal principal, decimal annualRate, int tenureMonths, DateTime startDate);
}
