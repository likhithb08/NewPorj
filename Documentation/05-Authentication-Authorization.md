# LOCPS - Authentication & Authorization

## Overview
LOCPS implements a comprehensive authentication and authorization system using ASP.NET Core Identity with claims-based authentication and role-based authorization.

## Authentication

### Authentication Flow

1. **User Login**
   - User submits credentials via `/Account/Login`
   - Credentials validated against database
   - Password verified using ASP.NET Core Identity PasswordHasher

2. **Cookie Creation**
   - Authentication cookie created with claims
   - Cookie name: `LOCPS.Auth`
   - Cookie duration: 8 hours
   - Secure and HttpOnly flags enabled

3. **Claims Added**
   - `ClaimTypes.NameIdentifier` → User ID
   - `ClaimTypes.Name` → Full Name
   - `ClaimTypes.Email` → Email
   - Custom claim `RoleToken` → Role token
   - Custom claim `RoleId` → Role ID

### Configuration

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = AuthConstants.CookieName;
        options.ExpireTimeSpan = TimeSpan.FromHours(AuthConstants.SessionHours);
        options.SlidingExpiration = true;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Set to Always in production
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });
```

### Constants

```csharp
public static class AuthConstants
{
    public const string CookieName = "LOCPS.Auth";
    public const string RoleDisplayCookieName = "locps_user_role";
    public const int SessionHours = 8;
}
```

## Authorization

### Role-Based Authorization

LOCPS uses a custom authorization filter `LocpsRoleAuthorizeFilter` to enforce role-based access control.

### Roles

| Role ID | Role Token | Description |
|---------|------------|-------------|
| 1 | ADMIN | System administrator |
| 2 | LOAN_OFFICER | Loan processing officer |
| 3 | UNDERWRITER | Credit underwriter |
| 4 | CUSTOMER | Loan applicant |

### Role Constants

```csharp
public static class RoleConstants
{
    // Role IDs
    public const int AdminId = 1;
    public const int LoanOfficerId = 2;
    public const int UnderwriterId = 3;
    public const int CustomerId = 4;

    // Role Tokens
    public const string AdminToken = "ADMIN";
    public const string LoanOfficerToken = "LOAN_OFFICER";
    public const string UnderwriterToken = "UNDERWRITER";
    public const string CustomerToken = "CUSTOMER";

    // Helper Methods
    public static string GetRoleFromId(int roleId) { ... }
    public static int GetIdFromRole(string roleToken) { ... }
}
```

### Route Protection

The custom filter checks:
1. User authentication status
2. Role claim from authentication cookie
3. Route prefix permission mapping
4. Fallback to legacy cookie for demo mode

### Permission Mapping

```csharp
private static readonly Dictionary<string, HashSet<string>> RoutePermissions = new()
{
    { "Admin", new HashSet<string> { "Product", "UserManagement", "Reports" } },
    { "LoanOfficer", new HashSet<string> { "Loan", "Kyc", "Credit", "Document" } },
    { "Underwriter", new HashSet<string> { "Approval", "Disbursement" } },
    { "Customer", new HashSet<string> { "Customer", "Dashboard" } }
};
```

### Filter Usage

```csharp
[ServiceFilter(typeof(LocpsRoleAuthorizeFilter))]
public class ProductController : Controller
{
    // Only accessible by Admin role
}
```

## Middleware Order

Critical middleware ordering in `Program.cs`:

```csharp
app.UseAuthentication();  // Must come before Authorization
app.UseAuthorization();
```

## Security Best Practices

### Current Implementation
- Password hashing with ASP.NET Core Identity
- Secure cookie flags (HttpOnly, SameSite)
- Sliding expiration for session management
- Role-based route protection
- Audit logging for authentication events

### Production Recommendations
1. Enable HTTPS only: `CookieSecurePolicy.Always`
2. Implement CSRF protection
3. Add rate limiting for login attempts
4. Implement account lockout after failed attempts
5. Add password complexity requirements
6. Implement multi-factor authentication
7. Use JWT tokens for API authentication
8. Add refresh token mechanism
9. Implement session fixation protection
10. Add security headers (CSP, X-Frame-Options, etc.)

## User Management

### User Creation
```csharp
var user = new User
{
    UserName = username,
    Email = email,
    FullName = fullName,
    PhoneNumber = phoneNumber,
    RoleId = roleId,
    IsActive = true,
    CreatedDate = DateTime.UtcNow
};
user.PasswordHash = _passwordHasher.HashPassword(user, password);
```

### Password Verification
```csharp
var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
if (result == PasswordVerificationResult.Failed)
{
    // Invalid password
}
```

### Role Assignment
```csharp
user.RoleId = roleId;
await _userRepository.UpdateUserAsync(user);
```

## Audit Logging

All authentication and authorization events are logged:

```csharp
await _auditLogService.LogAsync(
    userId, 
    Actions.Viewed, 
    "Login", 
    null, 
    email, 
    ipAddress, 
    userAgent
);
```

## Session Management

### Session Duration
- Default: 8 hours
- Configurable via `AuthConstants.SessionHours`
- Sliding expiration extends session on activity

### Session Termination
- User logout clears authentication cookie
- Cookie expiration after inactivity
- Manual session termination by admin

## Future Enhancements

1. **JWT Token Authentication**
   - Stateless authentication
   - Token refresh mechanism
   - API-only authentication

2. **OAuth 2.0 / OpenID Connect**
   - External identity providers
   - SSO integration
   - Social login support

3. **Advanced Security**
   - Multi-factor authentication
   - Biometric authentication
   - Device fingerprinting
   - Anomaly detection

4. **Compliance**
   - GDPR compliance features
   - Data retention policies
   - Consent management
   - Right to be forgotten
