using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces
{
    public interface IDisbursmentRepository : IGenericRepository<Disbursment>
    {
        Task<Disbursment> CreateDisbursmentAsync(Disbursment disbursment); // create a new Disbuesment record
        Task<Disbursment?> GetDisbursmentByApplicationIdAsync(int applicationId); //Finds the disbursement associated with a loan application
        Task<Disbursment> UpdateDisbursmentAsync(Disbursment disbursment);//Updates an existing disbursement
        Task<IEnumerable<Disbursment>> GetPendingDisbursmentsAsync(DisbursmentStatus status); //Retrieves disbursements having a specific status.

    }
}