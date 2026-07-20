using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class EmiService : IEmiService
{
    private readonly IEmiRepository _repository;
    private readonly IAuditLogService _auditLogService;

    public EmiService(IEmiRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    public async Task GenerateScheduleAsync(int applicationId, decimal principal, decimal annualRate, int tenureMonths, int createdByUserId)
    {
        var monthlyRate = annualRate / 12 / 100;
        var emiAmount = principal * monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, tenureMonths) /
                       ((decimal)Math.Pow(1 + (double)monthlyRate, tenureMonths) - 1);

        for (int i = 1; i <= tenureMonths; i++)
        {
            var emi = new Emi
            {
                ApplicationID = applicationId,
                EmiNumber = i,
                DueDate = DateTime.UtcNow.AddMonths(i),
                EmiAmount = Math.Round(emiAmount, 2),
                PaidAmount = 0,
                PenaltyAmount = 0,
                Status = EmiStatus.Pending
            };
            await _repository.AddAsync(emi);
        }

        await _auditLogService.LogAsync(createdByUserId, Actions.Created, $"EMISchedule:{applicationId}", null, $"Generated {tenureMonths} EMIs", null, null);
    }

    public async Task<IEnumerable<Emi>> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetByApplicationIdAsync(applicationId);

    public async Task<Emi> RecordPaymentAsync(int emiId, decimal paidAmount, int paidByUserId)
    {
        var emi = await _repository.GetByIdAsync(emiId)
            ?? throw new ServiceException("EMI not found.", 404);

        var oldStatus = emi.Status.ToString();
        emi.PaidAmount = paidAmount;
        emi.Status = EmiStatus.Paid;
        emi.PaidDate = DateTime.UtcNow;
        await _repository.UpdateAsync(emi);

        await _auditLogService.LogAsync(paidByUserId, Actions.Updated, $"EMI:{emiId}", oldStatus, "Paid", null, null);
        return emi;
    }
}
