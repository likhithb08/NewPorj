using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces
{
    public interface ILoanApplicationRepository : IGenericRepository<LoanApplication>
    {
        Task<LoanApplication?> CreateLoanApplicationAsync(LoanApplication application);
        Task<LoanApplication?> GetLoanApplicationByIdAsync(int id);
        Task<IEnumerable<LoanApplication>> GetAllLoanApplicationAsync();
        Task<LoanApplication?> GetLoanApplicationByCustomerIdAsync(int customerId);
        Task<LoanApplication?> UpdateLoanApplicationAsync(LoanApplication application);
        Task<LoanApplication?> DeleteLoanApplicationAsync(LoanApplication application);
        Task<IEnumerable<LoanApplication>> GetApplicationsByStatusAsync(ApplicationStatus status);
        Task<IEnumerable<LoanApplication>> GetApplicationsCreatedByUserId(int userId);
        Task<PagedResult<LoanApplication>> SearchAsync(PagedQuery query, ApplicationStatus? status = null, int? customerId = null);
        Task<LoanApplication?> GetWithDetailsAsync(int id);
    }
}
