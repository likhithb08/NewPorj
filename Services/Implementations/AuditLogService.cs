using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task LogAsync(int userId, Actions action, string entityId, string? oldValue, string? newValue, string? ipAddress, string? userAgent)
    {
        var log = new Auditlog
        {
            UserId = userId,
            Actions = action,
            EntityId = entityId,
            OldValue = oldValue ?? string.Empty,
            NewValue = newValue ?? string.Empty,
            IpAddress = ipAddress ?? string.Empty,
            UserAgent = userAgent ?? string.Empty,
            Timestamp = DateTime.UtcNow
        };

        await _repository.CreateAsync(log);
    }

    public async Task<PagedResult<Auditlog>> GetPagedAsync(PagedQuery query) =>
        await _repository.GetPagedAsync(query);

    public async Task<IEnumerable<Auditlog>> GetAuditLogsForApplicationAsync(int applicationId) =>
        await _repository.GetAuditLogsForApplicationAsync(applicationId);
}
