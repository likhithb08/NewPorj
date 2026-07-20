using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface IDisbursementService
{
    Task<Disbursment> CreateAsync(Disbursment disbursement);
    Task<Disbursment?> GetByApplicationIdAsync(int applicationId);
    Task<IEnumerable<Disbursment>> GetHistoryAsync();
    Task<Disbursment> ProcessAsync(int disbursementId, int processedByUserId);
}
