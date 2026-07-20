using LOCPS.Common;
using LOCPS.Data;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Repositories.Implementation;

public class AuditLogRepository : GenericRepository<Auditlog>, IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context) : base(context) => _context = context;

    public async Task<Auditlog> CreateAsync(Auditlog log)
    {
        log.Timestamp ??= DateTime.UtcNow;
        await _context.Auditlogs.AddAsync(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<PagedResult<Auditlog>> GetPagedAsync(PagedQuery query)
    {
        var q = _context.Auditlogs.Include(a => a.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            q = q.Where(a => a.EntityId.Contains(s) || a.User.FullName.Contains(s));
        }

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(a => a.Timestamp)
            .Skip((Math.Max(query.Page, 1) - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<Auditlog>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
    }

    public async Task<IEnumerable<Auditlog>> GetByUserIdAsync(int userId) =>
        await _context.Auditlogs.Where(a => a.UserId == userId).OrderByDescending(a => a.Timestamp).ToListAsync();
}

public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context) : base(context) => _context = context;

    public async Task<Document> CreateAsync(Document document)
    {
        await _context.Document.AddAsync(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<Document?> GetByIdWithDetailsAsync(int id) =>
        await _context.Document
            .Include(d => d.Application)
            .Include(d => d.Customer)
            .FirstOrDefaultAsync(d => d.DocumentId == id);

    public async Task<IEnumerable<Document>> GetByApplicationIdAsync(int applicationId) =>
        await _context.Document.Where(d => d.ApplicationId == applicationId).ToListAsync();

    public new async Task<Document> UpdateAsync(Document document)
    {
        _context.Document.Update(document);
        await _context.SaveChangesAsync();
        return document;
    }
}
