using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.ViewModels.Users;

namespace LOCPS.Services.Interfaces;

public interface IUserService
{
    Task<User> RegisterUserAsync(UserCreateViewModel model);
    Task<User?> LoginAsync(string email, string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetUsersByRoleAsync(Roles role);
    Task<User> UpdateUserAsync(UserUpdateViewModel model);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task<bool> AssignRoleAsync(int userId, int roleId);
}

public interface ILoanProductService
{
    Task<LoanProduct> CreateAsync(LoanProduct product, int createdByUserId);
    Task<LoanProduct?> GetByIdAsync(int id);
    Task<IEnumerable<LoanProduct>> GetAllAsync(bool activeOnly = true);
    Task<LoanProduct> UpdateAsync(LoanProduct product);
    Task<bool> DeleteAsync(int id);
}

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

public interface IKycService
{
    Task<Kyc> SubmitAsync(Kyc kyc);
    Task<Kyc?> GetByApplicationIdAsync(int applicationId);
    Task<Kyc> VerifyAsync(int kycId, int verifiedByUserId);
    Task<Kyc> RejectAsync(int kycId, int verifiedByUserId);
}

public interface ICreditEvaluationService
{
    Task<CreditEvaluation> CalculateAndSaveAsync(int applicationId, int evaluatedByUserId);
    Task<CreditEvaluation?> GetByApplicationIdAsync(int applicationId);
    Task<CreditEvaluation> ApproveAsync(int applicationId, int userId, string? comments);
    Task<CreditEvaluation> RejectAsync(int applicationId, int userId, string? comments);
}

public interface IApprovalService
{
    Task<Approval> ApproveLoanAsync(int applicationId, int approverUserId, decimal approvedAmount, int tenureMonths, decimal interestRate, string? comments);
    Task<Approval> RejectLoanAsync(int applicationId, int approverUserId, string reason, string? comments);
    Task<Approval?> GetByApplicationIdAsync(int applicationId);
    Task<IEnumerable<Approval>> GetHistoryAsync();
}

public interface IDisbursementService
{
    Task<Disbursment> CreateAsync(Disbursment disbursement);
    Task<Disbursment?> GetByApplicationIdAsync(int applicationId);
    Task<IEnumerable<Disbursment>> GetHistoryAsync();
    Task<Disbursment> ProcessAsync(int disbursementId, int processedByUserId);
}

public interface INotificationService
{
    Task<Notification> CreateAsync(int userId, NotificationType type, string title, string message, int? relatedApplicationId = null);
    Task<IEnumerable<Notification>> GetForUserAsync(int userId, bool unreadOnly = false);
    Task MarkAsReadAsync(int notificationId);
}

public interface IAuditLogService
{
    Task LogAsync(int userId, Actions action, string entityId, string? oldValue, string? newValue, string? ipAddress, string? userAgent);
    Task<PagedResult<Auditlog>> GetPagedAsync(PagedQuery query);
}

public interface IEmiService
{
    Task GenerateScheduleAsync(int applicationId, decimal principal, decimal annualRate, int tenureMonths, int createdByUserId);
    Task<IEnumerable<Emi>> GetByApplicationIdAsync(int applicationId);
    Task<Emi> RecordPaymentAsync(int emiId, decimal paidAmount, int paidByUserId);
}

public interface IDocumentService
{
    Task<Document> UploadAsync(Document document);
    Task<IEnumerable<Document>> GetByApplicationIdAsync(int applicationId);
    Task<Document> ApproveAsync(int documentId, int verifierUserId);
    Task<Document> RejectAsync(int documentId, int verifierUserId);
}

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(Roles role, int userId);
}

public class DashboardSummary
{
    public int TotalApplications { get; set; }
    public int PendingKyc { get; set; }
    public int PendingApprovals { get; set; }
    public int PendingDisbursements { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalUsers { get; set; }
}
