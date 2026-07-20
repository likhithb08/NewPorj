using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface ILoanProductService
{
    Task<LoanProduct> CreateAsync(LoanProduct product, int createdByUserId);
    Task<LoanProduct?> GetByIdAsync(int id);
    Task<IEnumerable<LoanProduct>> GetAllAsync(bool activeOnly = true);
    Task<LoanProduct> UpdateAsync(LoanProduct product);
    Task<bool> DeleteAsync(int id);
}
