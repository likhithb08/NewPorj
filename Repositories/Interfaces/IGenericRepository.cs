namespace LOCPS.Repositories.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        //bool because we are checking for a condition and returning true or false
        Task<bool> ExistsAsync(int id);

    }
}
