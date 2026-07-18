using System.Security.Claims;
using LOCPS.Constants;
using LOCPS.Enums;

namespace LOCPS.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(id, out var userId) ? userId : 0;
    }

    public static int GetRoleId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(RoleConstants.RoleIdClaimType);
        return int.TryParse(id, out var roleId) ? roleId : RoleConstants.CustomerRoleId;
    }

    public static Roles GetLocpsRole(this ClaimsPrincipal user) =>
        RoleConstants.GetRoleFromId(user.GetRoleId());

    public static string GetRoleToken(this ClaimsPrincipal user) =>
        user.FindFirstValue(RoleConstants.RoleClaimType)
        ?? RoleConstants.LoanOfficerToken;

    public static bool IsInLocpsRole(this ClaimsPrincipal user, params Roles[] roles) =>
        roles.Contains(user.GetLocpsRole());
}
