using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface IKycService
{
    Task<Kyc> SubmitAsync(Kyc kyc);
    Task<Kyc?> GetByApplicationIdAsync(int applicationId);
    Task<Kyc> VerifyAsync(int kycId, int verifiedByUserId, string remarks);
    Task<Kyc> RejectAsync(int kycId, int verifiedByUserId, string remarks);
    Task InitiateKycAsync(int applicationId, int loanOfficerId);
}
