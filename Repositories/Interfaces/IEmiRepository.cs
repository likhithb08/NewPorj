using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces
{
    public interface IEmiRepository : IGenericRepository<Emi>
    {
        /// <summary>
        /// Returns all EMIs for a specific loan application.
        /// </summary>
        Task<IEnumerable<Emi>> GetByApplicationIdAsync(int applicationId);
    }
}
