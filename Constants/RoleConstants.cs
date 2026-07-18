using LOCPS.Enums;

namespace LOCPS.Constants;

/// <summary>
/// Single source of truth for role IDs, enum values, cookie tokens, and claim names.
/// Must match <see cref="Migrations.SeedRolesData"/> migration seed data.
/// </summary>
public static class RoleConstants
{
    public const int CustomerRoleId = 1;
    public const int AdminRoleId = 2;
    public const int LoanOfficerRoleId = 3;
    public const int UnderWriterRoleId = 4;

    public const string RoleClaimType = "locps_role";
    public const string RoleIdClaimType = "RoleId";

    public const string CustomerToken = "customer";
    public const string AdminToken = "admin";
    public const string LoanOfficerToken = "officer";
    public const string UnderWriterToken = "underwriter";

    public static int GetRoleId(Roles role) => role switch
    {
        Roles.Customer => CustomerRoleId,
        Roles.Admin => AdminRoleId,
        Roles.LoanOfficer => LoanOfficerRoleId,
        Roles.UnderWriter => UnderWriterRoleId,
        _ => CustomerRoleId
    };

    public static Roles GetRoleFromId(int roleId) => roleId switch
    {
        CustomerRoleId => Roles.Customer,
        AdminRoleId => Roles.Admin,
        LoanOfficerRoleId => Roles.LoanOfficer,
        UnderWriterRoleId => Roles.UnderWriter,
        _ => Roles.Customer
    };

    public static string GetRoleToken(Roles role) => role switch
    {
        Roles.Customer => CustomerToken,
        Roles.Admin => AdminToken,
        Roles.LoanOfficer => LoanOfficerToken,
        Roles.UnderWriter => UnderWriterToken,
        _ => CustomerToken
    };

    public static string GetRoleToken(int roleId) => GetRoleToken(GetRoleFromId(roleId));

    public static string GetPolicyName(Roles role) => role switch
    {
        Roles.Customer => "CustomerOnly",
        Roles.Admin => "AdminOnly",
        Roles.LoanOfficer => "LoanOfficerOnly",
        Roles.UnderWriter => "UnderWriterOnly",
        _ => "CustomerOnly"
    };

    public static bool TryParseToken(string? token, out Roles role)
    {
        role = token?.ToLowerInvariant() switch
        {
            CustomerToken => Roles.Customer,
            AdminToken => Roles.Admin,
            LoanOfficerToken => Roles.LoanOfficer,
            UnderWriterToken => Roles.UnderWriter,
            _ => Roles.Customer
        };
        return !string.IsNullOrWhiteSpace(token);
    }
}
