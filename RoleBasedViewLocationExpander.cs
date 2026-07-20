using LOCPS.Constants;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Claims;

namespace LOCPS
{
    public class RoleBasedViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // First, try the authenticated user's RoleId claim (real auth)
            var roleIdClaim = context.ActionContext.HttpContext.User?.FindFirst(RoleConstants.RoleIdClaimType)?.Value;
            string normalizedRole = "Shared";

            if (!string.IsNullOrEmpty(roleIdClaim) && int.TryParse(roleIdClaim, out var roleId))
            {
                normalizedRole = roleId switch
                {
                    RoleConstants.AdminRoleId        => "Admin",
                    RoleConstants.CustomerRoleId     => "Customer",
                    RoleConstants.LoanOfficerRoleId  => "LoanOfficer",
                    RoleConstants.UnderWriterRoleId  => "Underwriter",
                    _                                => "Shared"
                };
            }
            else
            {
                // Fallback: check the legacy demo-mode cookie for unauthenticated preview
                var rawRole = context.ActionContext.HttpContext.Request.Cookies["locps_demo_role"] ?? string.Empty;
                normalizedRole = rawRole.ToLowerInvariant() switch
                {
                    "admin"       => "Admin",
                    "customer"    => "Customer",
                    "officer"     => "LoanOfficer",
                    "loanofficer" => "LoanOfficer",
                    "underwriter" => "Underwriter",
                    _             => "Shared"
                };
            }

            context.Values["locps_role"] = normalizedRole;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue("locps_role", out var role) && !string.IsNullOrEmpty(role))
            {
                yield return $"~/Roles/{role}/Views/{{1}}/{{0}}.cshtml";
                yield return $"~/Roles/{role}/Views/Shared/{{0}}.cshtml";
            }

            yield return "~/Roles/Shared/Views/{1}/{0}.cshtml";
            yield return "~/Roles/Shared/Views/Shared/{0}.cshtml";

            foreach (var location in viewLocations)
            {
                yield return location;
            }
        }
    }
}
