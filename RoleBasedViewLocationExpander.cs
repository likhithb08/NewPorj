using Microsoft.AspNetCore.Mvc.Razor;

namespace LOCPS
{
    public class RoleBasedViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var rawRole = context.ActionContext.HttpContext.Request.Cookies["locps_demo_role"] ?? "officer";
            var normalizedRole = rawRole.ToLowerInvariant() switch
            {
                "admin" => "Admin",
                "customer" => "Customer",
                "officer" => "LoanOfficer",
                "loanofficer" => "LoanOfficer",
                "underwriter" => "Underwriter",
                _ => "Shared"
            };

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
