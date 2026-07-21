using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(int userId, Actions action, string entityId, string? oldValue, string? newValue, string? ipAddress, string? userAgent);
    Task<PagedResult<Auditlog>> GetPagedAsync(PagedQuery query);
    Task<IEnumerable<Auditlog>> GetAuditLogsForApplicationAsync(int applicationId);
}
