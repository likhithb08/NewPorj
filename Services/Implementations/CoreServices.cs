using LOCPS.Common;
using LOCPS.Constants;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;
using LOCPS.ViewModels.Users;
using Microsoft.AspNetCore.Identity;

namespace LOCPS.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UserService(IUserRepository userRepository, IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _auditLogService = auditLogService;
    }

    public async Task<User> RegisterUserAsync(UserCreateViewModel model)
    {
        if (await _userRepository.GetUserByEmailIdAsync(model.Email) != null)
            throw new ServiceException("User already exists.", 409);

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            RoleId = model.RoleId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            PasswordHash = model.Password
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
        var created = await _userRepository.CreateUserAsync(user);
        await _auditLogService.LogAsync(created.UserId, Actions.Created, $"User:{created.UserId}", null, created.Email, null, null);
        return created;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ServiceException("Email is required.");
        var user = await _userRepository.GetUserByEmailIdAsync(email);
        if (user == null) throw new ServiceException("Invalid credentials.", 401);
        if (!user.IsActive) throw new ServiceException("Account is inactive.", 403);

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new ServiceException("Invalid credentials.", 401);

        user.LastLoginDate = DateTime.UtcNow;
        await _userRepository.UpdateUserAsync(user);
        await _auditLogService.LogAsync(user.UserId, Actions.Viewed, "Login", null, email, null, null);
        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        if (userId <= 0) throw new ServiceException("Invalid user ID.");
        return await _userRepository.GetUserByIdAsync(userId)
            ?? throw new ServiceException("User not found.", 404);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(Roles role)
    {
        if (!Enum.IsDefined(role)) throw new ServiceException("Invalid role.");
        return await _userRepository.GetUsersByRoleAsync(role);
    }

    public async Task<User> UpdateUserAsync(UserUpdateViewModel model)
    {
        var existing = await _userRepository.GetByIdAsync(model.UserId)
            ?? throw new ServiceException("User not found.", 404);

        existing.UserName = model.UserName;
        existing.Email = model.Email;
        existing.FullName = model.FullName;
        existing.PhoneNumber = model.PhoneNumber;
        existing.RoleId = model.RoleId;
        existing.IsActive = model.IsActive;

        var updated = await _userRepository.UpdateUserAsync(existing);
        await _auditLogService.LogAsync(updated.UserId, Actions.Updated, $"User:{updated.UserId}", null, updated.Email, null, null);
        return updated;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new ServiceException("User not found.", 404);
        await _userRepository.DeleteUserByIdAsync(user.UserId);
        await _auditLogService.LogAsync(userId, Actions.Updated, $"User:{userId}", user.Email, "Deleted", null, null);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new ServiceException("User not found.", 404);

        if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword) == PasswordVerificationResult.Failed)
            throw new ServiceException("Incorrect old password.");

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        await _userRepository.UpdateUserAsync(user);
        return true;
    }

    public async Task<bool> AssignRoleAsync(int userId, int roleId)
    {
        if (!Enum.IsDefined(typeof(Roles), RoleConstants.GetRoleFromId(roleId)))
            throw new ServiceException("Invalid role.");

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new ServiceException("User not found.", 404);

        user.RoleId = roleId;
        await _userRepository.UpdateUserAsync(user);
        return true;
    }
}

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

public class LoanApplicationService : ILoanApplicationService
{
    private readonly ILoanApplicationRepository _repository;
    private readonly ILoanProductRepository _productRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public LoanApplicationService(
        ILoanApplicationRepository repository,
        ILoanProductRepository productRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _repository = repository;
        _productRepository = productRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<LoanApplication> CreateAsync(LoanApplication application)
    {
        var product = await _productRepository.GetLoanProductByIdAsync(application.ProductId)
            ?? throw new ServiceException("Invalid loan product.", 400);

        if (application.RequestedAmount < product.MinAmount || application.RequestedAmount > product.MaxAmount)
            throw new ServiceException($"Amount must be between {product.MinAmount} and {product.MaxAmount}.");

        application.ApplicationNumber = GenerateApplicationNumber();
        application.Status = ApplicationStatus.Submitted;
        application.CreatedAt = DateTime.UtcNow;

        var created = await _repository.CreateLoanApplicationAsync(application)
            ?? throw new ServiceException("Failed to create application.");

        await _auditLogService.LogAsync(application.CreatedByUserId, Actions.Created, $"Application:{created.ApplicationId}", null, created.ApplicationNumber, null, null);
        await _notificationService.CreateAsync(application.CustomerId, NotificationType.ApplicationSubmitted, "Loan Application Submitted", $"Application {created.ApplicationNumber} submitted.", created.ApplicationId);
        return created;
    }

    public async Task<LoanApplication?> GetByIdAsync(int id) => await _repository.GetWithDetailsAsync(id);

    public Task<PagedResult<LoanApplication>> SearchAsync(PagedQuery query, ApplicationStatus? status = null, int? customerId = null) =>
        _repository.SearchAsync(query, status, customerId);

    public async Task<LoanApplication> UpdateAsync(LoanApplication application)
    {
        application.LastUpdatedDate = DateTime.UtcNow;
        return await _repository.UpdateLoanApplicationAsync(application)
            ?? throw new ServiceException("Application not found.", 404);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var app = await _repository.GetLoanApplicationByIdAsync(id)
            ?? throw new ServiceException("Application not found.", 404);
        await _repository.DeleteLoanApplicationAsync(app);
        return true;
    }

    public async Task<LoanApplication> UpdateStatusAsync(int applicationId, ApplicationStatus status, int actorUserId)
    {
        var app = await _repository.GetLoanApplicationByIdAsync(applicationId)
            ?? throw new ServiceException("Application not found.", 404);

        var old = app.Status.ToString();
        app.Status = status;
        app.LastUpdatedDate = DateTime.UtcNow;
        var updated = await _repository.UpdateLoanApplicationAsync(app)
            ?? throw new ServiceException("Update failed.");

        await _auditLogService.LogAsync(actorUserId, Actions.Updated, $"Application:{applicationId}", old, status.ToString(), null, null);
        return updated;
    }

    public string GenerateApplicationNumber() => $"LOC-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
}

public class KycService : IKycService
{
    private readonly IKycRepository _repository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public KycService(IKycRepository repository, IAuditLogService auditLogService, INotificationService notificationService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<Kyc> SubmitAsync(Kyc kyc)
    {
        kyc.VerificationStatus = KycStatus.Pending;
        kyc.CreatedDate = DateTime.UtcNow;
        kyc.IsActive = true;
        var created = await _repository.CreateKycAsync(kyc)
            ?? throw new ServiceException("Failed to submit KYC.");
        await _auditLogService.LogAsync(kyc.ApplicationId, Actions.Created, $"KYC:{created.KycId}", null, "Submitted", null, null);
        return created;
    }

    public async Task<Kyc?> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetKycByApplicationIdAsync(applicationId);

    public async Task<Kyc> VerifyAsync(int kycId, int verifiedByUserId)
    {
        var kyc = await _repository.GetByIdAsync(kycId)
            ?? throw new ServiceException("KYC not found.", 404);

        kyc.VerificationStatus = KycStatus.Verified;
        kyc.VerifiedByUserId = verifiedByUserId;
        kyc.VerifiedDate = DateTime.UtcNow;
        var updated = await _repository.UpdateKycAsync(kyc)
            ?? throw new ServiceException("Failed to verify KYC.");

        await _auditLogService.LogAsync(verifiedByUserId, Actions.Updated, $"KYC:{kycId}", "Pending", "Verified", null, null);
        await _notificationService.CreateAsync(kyc.ApplicationId, NotificationType.KYCVerified, "KYC Verified", "Your KYC has been verified successfully.", kyc.ApplicationId);
        return updated;
    }

    public async Task<Kyc> RejectAsync(int kycId, int verifiedByUserId)
    {
        var kyc = await _repository.GetByIdAsync(kycId)
            ?? throw new ServiceException("KYC not found.", 404);

        kyc.VerificationStatus = KycStatus.Rejected;
        kyc.VerifiedByUserId = verifiedByUserId;
        kyc.VerifiedDate = DateTime.UtcNow;
        var updated = await _repository.UpdateKycAsync(kyc)
            ?? throw new ServiceException("Failed to reject KYC.");

        await _auditLogService.LogAsync(verifiedByUserId, Actions.Updated, $"KYC:{kycId}", "Pending", "Rejected", null, null);
        return updated;
    }
}

public class CreditEvaluationService : ICreditEvaluationService
{
    private readonly ICreditEvaluationRepository _repository;
    private readonly ILoanApplicationRepository _applicationRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public CreditEvaluationService(
        ICreditEvaluationRepository repository,
        ILoanApplicationRepository applicationRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _repository = repository;
        _applicationRepository = applicationRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<CreditEvaluation> CalculateAndSaveAsync(int applicationId, int evaluatedByUserId)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(applicationId)
            ?? throw new ServiceException("Application not found.", 404);

        var evaluation = new CreditEvaluation
        {
            ApplicationId = applicationId,
            EvaluatedByUserId = evaluatedByUserId,
            EvaluatedDate = DateTime.UtcNow,
            CreditScore = CalculateCreditScore(application),
            DebitToIncomeRatio = CalculateDTI(application),
            PaymentHistoryScore = 85,
            ExistingLiabilities = 0,
            CreditRecommendation = CreditRecommendation.Pending
        };

        var created = await _repository.CreateCreditEvaluationAsync(evaluation);
        await _auditLogService.LogAsync(evaluatedByUserId, Actions.Created, $"CreditEval:{created.CreditId}", null, "Calculated", null, null);
        return created;
    }

    public async Task<CreditEvaluation?> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetCreditEvaluationByApplicationAsync(applicationId);

    public async Task<CreditEvaluation> ApproveAsync(int applicationId, int userId, string? comments)
    {
        var eval = await _repository.GetCreditEvaluationByApplicationAsync(applicationId)
            ?? throw new ServiceException("Credit evaluation not found.", 404);

        eval.CreditRecommendation = CreditRecommendation.Approved;
        eval.Comments = comments;
        eval.EvaluatedDate = DateTime.UtcNow;
        var updated = await _repository.UpdateCreditEvaluationAsync(eval);

        await _auditLogService.LogAsync(userId, Actions.Updated, $"CreditEval:{eval.CreditId}", "Pending", "Approved", null, null);
        await _notificationService.CreateAsync(applicationId, NotificationType.CreditEvaluated, "Credit Evaluation Approved", "Your credit evaluation has been approved.", applicationId);
        return updated;
    }

    public async Task<CreditEvaluation> RejectAsync(int applicationId, int userId, string? comments)
    {
        var eval = await _repository.GetCreditEvaluationByApplicationAsync(applicationId)
            ?? throw new ServiceException("Credit evaluation not found.", 404);

        eval.CreditRecommendation = CreditRecommendation.Rejected;
        eval.Comments = comments;
        eval.EvaluatedDate = DateTime.UtcNow;
        var updated = await _repository.UpdateCreditEvaluationAsync(eval);

        await _auditLogService.LogAsync(userId, Actions.Updated, $"CreditEval:{eval.CreditId}", "Pending", "Rejected", null, null);
        return updated;
    }

    private int CalculateCreditScore(LoanApplication application)
    {
        var score = 600;
        if (application.AnnualIncome > 500000) score += 100;
        else if (application.AnnualIncome > 300000) score += 50;
        return Math.Min(900, score);
    }

    private decimal CalculateDTI(LoanApplication application)
    {
        return application.RequestedAmount > 0 ? (application.RequestedAmount / (application.AnnualIncome * 12)) * 100 : 0;
    }
}

public class ApprovalService : IApprovalService
{
    private readonly IApprovalRepository _repository;
    private readonly ILoanApplicationRepository _applicationRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public ApprovalService(
        IApprovalRepository repository,
        ILoanApplicationRepository applicationRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _repository = repository;
        _applicationRepository = applicationRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<Approval> ApproveLoanAsync(int applicationId, int approverUserId, decimal approvedAmount, int tenureMonths, decimal interestRate, string? comments)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(applicationId)
            ?? throw new ServiceException("Application not found.", 404);

        var approval = new Approval
        {
            ApplicationId = applicationId,
            ApprovedByUserId = approverUserId,
            ApprovedAmount = approvedAmount,  // Bug 6 fix: was cast to (long) which truncated decimal cents
            ApprovedInterestRate = interestRate,
            ApprovedTenureMonths = tenureMonths,
            ApprovalStatus = ApprovalStatus.Approved,
            ApprovalDate = DateTime.UtcNow,
            Comments = comments ?? string.Empty
        };

        var created = await _repository.CreateApprovalAsync(approval)
            ?? throw new ServiceException("Failed to approve loan.");

        application.Status = ApplicationStatus.Approved;
        application.ApprovedAmount = approvedAmount;
        await _applicationRepository.UpdateLoanApplicationAsync(application);

        await _auditLogService.LogAsync(approverUserId, Actions.Updated, $"Approval:{created.ApprovalId}", null, "Approved", null, null);
        await _notificationService.CreateAsync(application.CustomerId, NotificationType.ApprovalUpdate, "Loan Approved", $"Your loan of {approvedAmount} has been approved.", applicationId);
        return created;
    }

    public async Task<Approval> RejectLoanAsync(int applicationId, int approverUserId, string reason, string? comments)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(applicationId)
            ?? throw new ServiceException("Application not found.", 404);

        var approval = new Approval
        {
            ApplicationId = applicationId,
            ApprovedByUserId = approverUserId,
            ApprovalStatus = ApprovalStatus.Rejected,
            ApprovalDate = DateTime.UtcNow,
            Comments = comments ?? string.Empty,
            RejectionReason = reason
        };

        var created = await _repository.CreateApprovalAsync(approval)
            ?? throw new ServiceException("Failed to reject loan.");

        application.Status = ApplicationStatus.Rejected;
        await _applicationRepository.UpdateLoanApplicationAsync(application);

        await _auditLogService.LogAsync(approverUserId, Actions.Updated, $"Approval:{created.ApprovalId}", null, "Rejected", null, null);
        await _notificationService.CreateAsync(application.CustomerId, NotificationType.ApprovalUpdate, "Loan Rejected", $"Your loan application has been rejected. Reason: {reason}", applicationId);
        return created;
    }

    public async Task<Approval?> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetApprovalByApplicationIdAsync(applicationId);

    public async Task<IEnumerable<Approval>> GetHistoryAsync() =>
        await _repository.GetAllAsync();
}

public class DisbursementService : IDisbursementService
{
    private readonly IDisbursmentRepository _repository;
    private readonly ILoanApplicationRepository _applicationRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public DisbursementService(
        IDisbursmentRepository repository,
        ILoanApplicationRepository applicationRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _repository = repository;
        _applicationRepository = applicationRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    public async Task<Disbursment> CreateAsync(Disbursment disbursement)
    {
        disbursement.DisbursmentDate = DateTime.UtcNow;
        disbursement.Status = DisbursmentStatus.Pending;
        var created = await _repository.CreateDisbursmentAsync(disbursement)
            ?? throw new ServiceException("Failed to create disbursement.");
        await _auditLogService.LogAsync(disbursement.ProcessedByUserID, Actions.Created, $"Disbursement:{created.DisbursmentId}", null, "Created", null, null);
        return created;
    }

    public async Task<Disbursment?> GetByApplicationIdAsync(int applicationId) =>
        await _repository.GetDisbursmentByApplicationIdAsync(applicationId);

    public async Task<IEnumerable<Disbursment>> GetHistoryAsync() =>
        await _repository.GetPendingDisbursmentsAsync(DisbursmentStatus.Completed);

    public async Task<Disbursment> ProcessAsync(int disbursementId, int processedByUserId)
    {
        var disbursement = await _repository.GetByIdAsync(disbursementId)
            ?? throw new ServiceException("Disbursement not found.", 404);

        disbursement.Status = DisbursmentStatus.Completed;
        disbursement.ProcessedByUserID = processedByUserId;
        disbursement.DisbursmentDate = DateTime.UtcNow;
        var updated = await _repository.UpdateDisbursmentAsync(disbursement)
            ?? throw new ServiceException("Failed to process disbursement.");

        var application = await _applicationRepository.GetWithDetailsAsync(disbursement.ApplicationId);
        if (application != null)
        {
            application.Status = ApplicationStatus.Disbursed;
            await _applicationRepository.UpdateLoanApplicationAsync(application);
            await _notificationService.CreateAsync(application.CustomerId, NotificationType.DisbursementProcessed, "Disbursement Processed", $"Your loan disbursement of {disbursement.AmountApproved} has been processed.", application.ApplicationId);
        }

        await _auditLogService.LogAsync(processedByUserId, Actions.Updated, $"Disbursement:{disbursementId}", "Pending", "Completed", null, null);
        return updated;
    }
}

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Notification> CreateAsync(int userId, NotificationType type, string title, string message, int? relatedApplicationId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            NotificationType = type,
            Title = title,
            Message = message,
            RelatedApplicationId = relatedApplicationId ?? 0,
            IsRead = false,
            CreatedDate = DateTime.UtcNow
        };

        return await _repository.CreateNotificationAsync(notification)
            ?? throw new ServiceException("Failed to create notification.");
    }

    public async Task<IEnumerable<Notification>> GetForUserAsync(int userId, bool unreadOnly = false)
    {
        // Bug 4 fix: was wrapping a single Notification? in a List — user saw only 1 notification ever
        var notifications = await _repository.GetNotificationByUserIdAsync(userId);
        return unreadOnly ? notifications.Where(n => !n.IsRead) : notifications;
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _repository.GetByIdAsync(notificationId)
            ?? throw new ServiceException("Notification not found.", 404);
        notification.IsRead = true;
        await _repository.MarkAsReadAsync(notification);
    }
}

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task LogAsync(int userId, Actions action, string entityId, string? oldValue, string? newValue, string? ipAddress, string? userAgent)
    {
        var log = new Auditlog
        {
            UserId = userId,
            Actions = action,
            EntityId = entityId,
            OldValue = oldValue ?? string.Empty,
            NewValue = newValue ?? string.Empty,
            IpAddress = ipAddress ?? string.Empty,
            UserAgent = userAgent ?? string.Empty,
            Timestamp = DateTime.UtcNow
        };

        await _repository.CreateAsync(log);
    }

    public async Task<PagedResult<Auditlog>> GetPagedAsync(PagedQuery query) =>
        await _repository.GetPagedAsync(query);
}

public class EmiService : IEmiService
{
    // Bug 3 fix: was IGenericRepository<Emi> which had no filtered query — caused ALL EMIs to be returned
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
        // Bug 5 fix: EmiAmount is now decimal — removed (int) cast that was truncating fractional amounts
        var emiAmount = principal * monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, tenureMonths) /
                       ((decimal)Math.Pow(1 + (double)monthlyRate, tenureMonths) - 1);

        for (int i = 1; i <= tenureMonths; i++)
        {
            var emi = new Emi
            {
                ApplicationID = applicationId,
                EmiNumber = i,
                DueDate = DateTime.UtcNow.AddMonths(i),
                EmiAmount = Math.Round(emiAmount, 2),  // decimal — no truncation
                PaidAmount = 0,
                PenaltyAmount = 0,
                Status = EmiStatus.Pending
            };
            await _repository.AddAsync(emi);
        }

        await _auditLogService.LogAsync(createdByUserId, Actions.Created, $"EMISchedule:{applicationId}", null, $"Generated {tenureMonths} EMIs", null, null);
    }

    public async Task<IEnumerable<Emi>> GetByApplicationIdAsync(int applicationId)
    {
        // Bug 3 fix: was GetAllAsync() — returns ALL EMIs for all loans, now filtered by applicationId
        return await _repository.GetByApplicationIdAsync(applicationId);
    }

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
