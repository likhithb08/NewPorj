using LOCPS.DTOs;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace LOCPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserApiController : ControllerBase
{
    private readonly IUserServices _userService;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UserApiController(IUserServices userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResult<UserDto>>> Register([FromBody] CreateUserDto model)
    {
        try
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = model.Password,
                RoleId = model.RoleId
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            var created = await _userService.RegisterUserAsync(user);
            var userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };

            return Ok(ApiResult<UserDto>.Ok(userDto, "User registered successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<UserDto>.Fail(ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResult<UserDto>>> Login([FromBody] LoginDto model)
    {
        try
        {
            var user = await _userService.LoginAsync(model.Email, model.Password);
            var userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };

            return Ok(ApiResult<UserDto>.Ok(userDto, "Login successful"));
        }
        catch (Exception ex)
        {
            return Unauthorized(ApiResult<UserDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResult<UserDto>>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            var userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };

            return Ok(ApiResult<UserDto>.Ok(userDto));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResult<UserDto>.Fail(ex.Message));
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<List<UserDto>>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedDate = u.CreatedDate
            }).ToList();

            return Ok(ApiResult<List<UserDto>>.Ok(userDtos));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<List<UserDto>>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResult<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto model)
    {
        try
        {
            var user = new User
            {
                UserId = id,
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                RoleId = model.RoleId,
                IsActive = model.IsActive
            };

            var updated = await _userService.UpdateUserAsync(user);
            var userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };

            return Ok(ApiResult<UserDto>.Ok(userDto, "User updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<UserDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResult>> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(ApiResult.Ok("User deleted successfully"));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResult.Fail(ex.Message));
        }
    }
}
