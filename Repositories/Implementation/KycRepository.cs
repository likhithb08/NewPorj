using LOCPS.Repositories.Interfaces;
using LOCPS.Models;
using LOCPS.Data;
using LOCPS.Enums;
using Microsoft.EntityFrameworkCore;
namespace LOCPS.Repositories.Implementation
{
    public class KycRepository : GenericRepository<Kyc>, IKycRepository
    {
        private readonly AppDbContext _context;
        public KycRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Kyc> CreateKycAsync(Kyc kyc)
        {
            await _context.Kyc.AddAsync(kyc);
            await _context.SaveChangesAsync();
            return kyc;
        }
        public async Task<Kyc?> GetKycByApplicationIdAsync(int applicationId)
        {
            return await _context.Kyc.FirstOrDefaultAsync(k => k.ApplicationId == applicationId);
        }

        public async Task<Kyc> UpdateKycAsync(Kyc kyc)
        {
            _context.Kyc.Update(kyc);
            await _context.SaveChangesAsync();
            return kyc;
        }

        public async Task<IEnumerable<Kyc>> GetPendingKycAsync(KycStatus status)
        {
            return await _context.Kyc.Where(k => k.VerificationStatus == status).ToListAsync();
        }

    }
}
