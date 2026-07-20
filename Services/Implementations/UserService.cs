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
