using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces
{
    public interface IApprovalRepository : IGenericRepository<Approval>
    {
        Task<Approval> CreateApprovalAsync(Approval approval);
        Task<Approval?> GetApprovalByApplicationIdAsync(int applicationId);
        Task<Approval> ApproveLoanAsync(int applicationId);
        Task<Approval> RejectLoanAsync(int applicationId);
        Task<Approval> UpdateApprovalAsync(Approval approval);

    }
}
