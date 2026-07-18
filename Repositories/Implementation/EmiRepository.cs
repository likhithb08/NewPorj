using LOCPS.Data;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Repositories.Implementation
{
    public class EmiRepository : GenericRepository<Emi>, IEmiRepository
    {
        private readonly AppDbContext _context;

        public EmiRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all EMIs filtered by the given applicationId.
        /// Previously, EmiService called GetAllAsync() which returned every EMI for every loan.
        /// </summary>
        public async Task<IEnumerable<Emi>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Emis
                .Where(e => e.ApplicationID == applicationId)
                .OrderBy(e => e.EmiNumber)
                .ToListAsync();
        }
    }
}
