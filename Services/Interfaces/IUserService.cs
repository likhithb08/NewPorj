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
