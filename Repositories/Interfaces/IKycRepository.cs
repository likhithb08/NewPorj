using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces
{
    public interface IKycRepository : IGenericRepository<Kyc>
    {
        Task<Kyc> CreateKycAsync(Kyc kyc);
        Task<Kyc?> GetKycByApplicationIdAsync(int applicationId);
        Task<Kyc> UpdateKycAsync(Kyc kyc);
        Task<IEnumerable<Kyc>> GetPendingKycAsync(KycStatus status);

    }
}
