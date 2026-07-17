using LOCPS.Repositories.Interfaces;
using LOCPS.Models;
using LOCPS.Data;
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
            return await _context.Disbursments.FirstOrDefaultAsync(d => d.ApplicationId == applicationId);
        }
        public async Task<Disbursment> UpdateDisbursmentAsync(Disbursment disbursmeant)
        {
            _context.Disbursments.Update(disbursmeant);
            await _context.SaveChangesAsync();
            return disbursmeant;
        }
        public async Task<IEnumerable<Disbursment>> GetPendingDisbursmentsAsync(DisbursmentStatus status)
        {
            return await _context.Disbursments.Where(d => d.Status == status).ToListAsync();
        }
    }
}