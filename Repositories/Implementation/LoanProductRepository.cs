using LOCPS.Data;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Repositories.Implementation
{
    public class LoanProductRepository : GenericRepository<LoanProduct>, ILoanProductRepository
    {
        private readonly AppDbContext _context;

        public LoanProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<LoanProduct?> CreateLoanProductAsync(LoanProduct product)
        {
            await _context.LoanProducts.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<LoanProduct>> GetAllLoanProductAsync()
        {
            return await _context.LoanProducts.ToListAsync();
        }

        public async Task<LoanProduct?> GetLoanProductByIdAsync(int id)
        {
            return await _context.LoanProducts.FirstOrDefaultAsync(i => i.ProductId == id);

        }

        public async Task<LoanProduct?> UpdateLoanProductAsync(LoanProduct product)
        {
            _context.LoanProducts.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteLoanProductAsync(LoanProduct product)
        {
            _context.LoanProducts.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
