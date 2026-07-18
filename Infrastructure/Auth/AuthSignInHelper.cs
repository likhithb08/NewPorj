using System.Security.Claims;
using LOCPS.Constants;
using LOCPS.Enums;
using LOCPS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LOCPS.Infrastructure.Auth;

public static class AuthSignInHelper
{
    public static async Task SignInUserAsync(HttpContext httpContext, User user)
    {
        var role = RoleConstants.GetRoleFromId(user.RoleId);
        var roleToken = RoleConstants.GetRoleToken(role);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, role.ToString()),
            new(RoleConstants.RoleClaimType, roleToken),
            new(RoleConstants.RoleIdClaimType, user.RoleId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(AuthConstants.SessionHours)
            });

        httpContext.Response.Cookies.Append(AuthConstants.RoleDisplayCookieName, roleToken, new CookieOptions
        {
            Path = "/",
            HttpOnly = false,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromHours(AuthConstants.SessionHours)
        });
    }

    public static async Task SignOutUserAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        httpContext.Response.Cookies.Delete(AuthConstants.RoleDisplayCookieName);
    }
}
