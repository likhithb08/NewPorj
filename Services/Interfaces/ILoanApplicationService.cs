using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface ILoanApplicationService
{
    Task<LoanApplication> CreateAsync(LoanApplication application);
    Task<LoanApplication?> GetByIdAsync(int id);
    Task<PagedResult<LoanApplication>> SearchAsync(PagedQuery query, ApplicationStatus? status = null, int? customerId = null);
    Task<LoanApplication> UpdateAsync(LoanApplication application);
    Task<bool> DeleteAsync(int id);
    Task<LoanApplication> UpdateStatusAsync(int applicationId, ApplicationStatus status, int actorUserId);
    string GenerateApplicationNumber();
}
