using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class LoanProductService : ILoanProductService
{
    private readonly ILoanProductRepository _repository;
    private readonly IAuditLogService _auditLogService;

    public LoanProductService(ILoanProductRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    public async Task<LoanProduct> CreateAsync(LoanProduct product, int createdByUserId)
    {
        product.CreatedByUserId = createdByUserId;
        product.CreatedAt = DateTime.UtcNow;
        product.IsActive = true;
        var created = await _repository.CreateLoanProductAsync(product)
            ?? throw new ServiceException("Failed to create product.");
        await _auditLogService.LogAsync(createdByUserId, Actions.Created, $"Product:{created.ProductId}", null, created.ProductName, null, null);
        return created;
    }

    public async Task<LoanProduct?> GetByIdAsync(int id) => await _repository.GetLoanProductByIdAsync(id);

    public async Task<IEnumerable<LoanProduct>> GetAllAsync(bool activeOnly = true)
    {
        var items = await _repository.GetAllLoanProductAsync();
        return activeOnly ? items.Where(p => p.IsActive) : items;
    }

    public async Task<LoanProduct> UpdateAsync(LoanProduct product)
    {
        var updated = await _repository.UpdateLoanProductAsync(product)
            ?? throw new ServiceException("Product not found.", 404);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _repository.GetLoanProductByIdAsync(id)
            ?? throw new ServiceException("Product not found.", 404);
        product.IsActive = false;
        await _repository.UpdateLoanProductAsync(product);
        return true;
    }
}
