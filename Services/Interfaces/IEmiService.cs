using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface IEmiService
{
    Task GenerateScheduleAsync(int applicationId, decimal principal, decimal annualRate, int tenureMonths, int createdByUserId);
    Task<IEnumerable<Emi>> GetByApplicationIdAsync(int applicationId);
    Task<Emi> RecordPaymentAsync(int emiId, decimal paidAmount, int paidByUserId);
}
