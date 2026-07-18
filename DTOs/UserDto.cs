using System.ComponentModel.DataAnnotations;

namespace LOCPS.DTOs;

public class UserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateUserDto
{
    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }
}

public class UpdateUserDto
{
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public bool IsActive { get; set; }
}

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
