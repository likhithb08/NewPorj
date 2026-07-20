using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface IApprovalService
{
    Task<Approval> ApproveLoanAsync(int applicationId, int approverUserId, decimal approvedAmount, int tenureMonths, decimal interestRate, string? comments);
    Task<Approval> RejectLoanAsync(int applicationId, int approverUserId, string reason, string? comments);
    Task<Approval?> GetByApplicationIdAsync(int applicationId);
    Task<IEnumerable<Approval>> GetHistoryAsync();
}
