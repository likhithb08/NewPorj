using System.ComponentModel.DataAnnotations;

namespace LOCPS.ViewModels.Users;

public class UserCreateViewModel
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Range(1, 4)]
    public int RoleId { get; set; } = 1;
}

public class UserUpdateViewModel
{
    public int UserId { get; set; }

    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UserListViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class UserDetailsViewModel : UserListViewModel
{
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? LastLoginDate { get; set; }
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}


