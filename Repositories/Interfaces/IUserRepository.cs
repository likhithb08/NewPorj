using LOCPS.Enums;
using LOCPS.Models;
namespace LOCPS.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailIdAsync(string email);

        Task<IEnumerable<User?>> GetUsersByRoleAsync(Roles role);

        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserByIdAsync(int userId);
    }
}
