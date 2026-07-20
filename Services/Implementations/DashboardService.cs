using LOCPS.Enums;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly ILoanApplicationRepository _applicationRepository;
    private readonly IKycRepository _kycRepository;
    private readonly IApprovalRepository _approvalRepository;
    private readonly IDisbursmentRepository _disbursmentRepository;
    private readonly ILoanProductRepository _loanProductRepository;
    private readonly IUserRepository _userRepository;

    public DashboardService(
        ILoanApplicationRepository applicationRepository,
        IKycRepository kycRepository,
        IApprovalRepository approvalRepository,
        IDisbursmentRepository disbursmentRepository,
        ILoanProductRepository loanProductRepository,
        IUserRepository userRepository)
    {
        _applicationRepository = applicationRepository;
        _kycRepository = kycRepository;
        _approvalRepository = approvalRepository;
        _disbursmentRepository = disbursmentRepository;
        _loanProductRepository = loanProductRepository;
        _userRepository = userRepository;
    }

    public async Task<DashboardSummary> GetSummaryAsync(Roles role, int userId)
    {
        var summary = new DashboardSummary();

        // 1. Total applications depending on role
        var allApplications = await _applicationRepository.GetAllAsync();
        if (role == Roles.Customer)
        {
            summary.TotalApplications = allApplications.Count(a => a.CustomerId == userId);
        }
        else
        {
            summary.TotalApplications = allApplications.Count();
        }

        // 2. Pending KYC
        var allKyc = await _kycRepository.GetAllAsync();
        summary.PendingKyc = allKyc.Count(k => k.VerificationStatus == KycStatus.Pending);

        // 3. Pending Approvals (Status is KYCVerified or UnderReview)
        summary.PendingApprovals = allApplications.Count(a => a.Status == ApplicationStatus.KYCVerified || a.Status == ApplicationStatus.UnderReview);

        // 4. Pending Disbursements (Status is Approved)
        summary.PendingDisbursements = allApplications.Count(a => a.Status == ApplicationStatus.Approved);

        // 5. Active products
        var products = await _loanProductRepository.GetAllLoanProductAsync();
        summary.ActiveProducts = products.Count(p => p.IsActive);

        // 6. Total Users
        var users = await _userRepository.GetAllAsync();
        summary.TotalUsers = users.Count();

        return summary;
    }
}
