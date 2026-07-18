using LOCPS.Common;
using LOCPS.Data;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Repositories.Implementation
{
    public class LoanApplicationRepository : GenericRepository<LoanApplication>, ILoanApplicationRepository
    {
        private readonly AppDbContext _context;
        public LoanApplicationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<LoanApplication?> CreateLoanApplicationAsync(LoanApplication application)
        {
            await _context.LoanApplications.AddAsync(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<LoanApplication?> GetLoanApplicationByIdAsync(int id)
        {
            return await _context.LoanApplications.FindAsync(id);
        }

        public async Task<IEnumerable<LoanApplication>> GetAllLoanApplicationAsync()
        {
            return await _context.LoanApplications.ToListAsync();
        }

        public async Task<LoanApplication?> GetLoanApplicationByCustomerIdAsync(int customerId)
        {
            return await _context.LoanApplications.FirstOrDefaultAsync(i => i.CustomerId == customerId);
        }

        public async Task<LoanApplication?> UpdateLoanApplicationAsync(LoanApplication application)
        {
            _context.LoanApplications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<LoanApplication?> DeleteLoanApplicationAsync(LoanApplication application)
        {
            _context.LoanApplications.Remove(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<IEnumerable<LoanApplication>> GetApplicationsByStatusAsync(ApplicationStatus status)
        {
            return await _context.LoanApplications.Where(asp => asp.Status == status).ToListAsync(); 
        }

        public async Task<IEnumerable<LoanApplication>> GetApplicationsCreatedByUserId(int userId)
        {
            return await _context.LoanApplications.Where(ui => ui.CreatedByUserId == userId).ToListAsync();
        }

        public async Task<PagedResult<LoanApplication>> SearchAsync(PagedQuery query, ApplicationStatus? status = null, int? customerId = null)
        {
            var q = _context.LoanApplications
                .Include(la => la.Customer)
                .Include(la => la.Product)
                .AsQueryable();

            if (status.HasValue)
                q = q.Where(la => la.Status == status.Value);
            if (customerId.HasValue)
                q = q.Where(la => la.CustomerId == customerId.Value);
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(la =>
                    (la.ApplicationNumber != null && la.ApplicationNumber.Contains(s)) ||
                    la.Customer.FullName.Contains(s) ||
                    la.Product.ProductName.Contains(s));
            }

            q = query.SortBy?.ToLowerInvariant() switch
            {
                "amount" => query.SortDescending ? q.OrderByDescending(la => la.RequestedAmount) : q.OrderBy(la => la.RequestedAmount),
                "date" => query.SortDescending ? q.OrderByDescending(la => la.CreatedAt) : q.OrderBy(la => la.CreatedAt),
                _ => q.OrderByDescending(la => la.CreatedAt)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((Math.Max(query.Page, 1) - 1) * query.PageSize).Take(query.PageSize).ToListAsync();
            return new PagedResult<LoanApplication> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalCount = total };
        }

        public async Task<LoanApplication?> GetWithDetailsAsync(int id) =>
            await _context.LoanApplications
                .Include(la => la.Customer)
                .Include(la => la.Product)
                .Include(la => la.CreatedBy)
                .FirstOrDefaultAsync(la => la.ApplicationId == id);
    }
}
