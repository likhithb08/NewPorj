using LOCPS.Repositories.Interfaces;
using LOCPS.Models;
using LOCPS.Data;
using LOCPS.Enums;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Repositories.Implementation
{
    public class DisbursmentRepository : GenericRepository<Disbursment>, IDisbursmentRepository
    {
        private readonly AppDbContext _context;
        public DisbursmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Disbursment> CreateDisbursmentAsync(Disbursment disbursmeant)
        {
            await _context.Disbursments.AddAsync(disbursmeant);
            await _context.SaveChangesAsync();
            return disbursmeant;
        }
        public async Task<Disbursment?> GetDisbursmentByApplicationIdAsync(int applicationId)
        {
            return await _context.Disbursments
                .Include(d => d.LoanApplication)
                    .ThenInclude(la => la.Customer)
                .FirstOrDefaultAsync(d => d.ApplicationId == applicationId);
        }
        public async Task<Disbursment> UpdateDisbursmentAsync(Disbursment disbursmeant)
        {
            _context.Disbursments.Update(disbursmeant);
            await _context.SaveChangesAsync();
            return disbursmeant;
        }
        public async Task<IEnumerable<Disbursment>> GetPendingDisbursmentsAsync(DisbursmentStatus status)
        {
            return await _context.Disbursments
                .Include(d => d.LoanApplication)
                    .ThenInclude(la => la.Customer)
                .Where(d => d.Status == status)
                .OrderByDescending(d => d.DisbursmentDate)
                .ToListAsync();
        }
    }
}