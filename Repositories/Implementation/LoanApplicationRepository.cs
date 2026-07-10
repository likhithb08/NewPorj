using LOCPS.Data;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
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
    }
}
