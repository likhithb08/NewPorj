using LOCPS.Data;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Repositories.Implementation
{
    public class ApprovalRepository : GenericRepository<Approval>, IApprovalRepository
    {
        private readonly AppDbContext _context;
        public ApprovalRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Approval> CreateApprovalAsync(Approval approval)
        {
            await _context.Approval.AddAsync(approval);
            await _context.SaveChangesAsync();
            return approval;
        }

        public async Task<Approval?> GetApprovalByApplicationIdAsync(int applicationId)
        {
            return await _context.Approval.FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }
        public async Task<Approval> ApproveLoanAsync(int applicationId)
        {
            var approval = await GetApprovalByApplicationIdAsync(applicationId);
            if (approval != null)
            {
                approval.ApprovalStatus = ApprovalStatus.Approved;
                await _context.SaveChangesAsync();
            }
            return approval!;
        }
            public async Task<Approval> RejectLoanAsync(int applicationId)
        {
            var approval = await GetApprovalByApplicationIdAsync(applicationId);
            if (approval != null)
            {
                approval.ApprovalStatus = ApprovalStatus.Rejected;
                await _context.SaveChangesAsync();
            }
            return approval!;
        }

        public async Task<Approval> UpdateApprovalAsync(Approval approval)
        {
            _context.Approval.Update(approval);
            await _context.SaveChangesAsync();
            return approval;
        }
    }
}
