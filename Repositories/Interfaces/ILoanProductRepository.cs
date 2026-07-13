using LOCPS.Models;
using System.Collections.Generic;

namespace LOCPS.Repositories.Interfaces
{
    public interface ILoanProductRepository : IGenericRepository<LoanProduct>
    {
        Task<IEnumerable<LoanProduct>> GetAllLoanProductAsync();
        Task<LoanProduct?> GetLoanProductByIdAsync(int id);
        Task<LoanProduct?> CreateLoanProductAsync(LoanProduct product);
        Task<LoanProduct?> UpdateLoanProductAsync(LoanProduct product);
        Task DeleteLoanProductAsync(LoanProduct product);
    }
}
