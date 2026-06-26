using Microsoft.AspNetCore.Mvc.Razor;

namespace LOCPS
{
    public class RoleBasedViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var role = context.ActionContext.HttpContext.Request.Cookies["locps_demo_role"] ?? "officer";
            context.Values["locps_role"] = role.ToLowerInvariant();
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue("locps_role", out var role) && !string.IsNullOrEmpty(role))
            {
                var folder = role switch
                {
                    "officer" => "LoanOfficer",
                    "customer" => "Customer",
                    "underwriter" => "Underwriter",
                    "admin" => "Admin",
                    _ => "Shared"
                };

                yield return $"/Roles/{folder}/Views/{{1}}/{{0}}.cshtml";
                yield return $"/Roles/{folder}/Views/{{0}}.cshtml";
                yield return $"/Roles/{folder}/Views/Shared/{{0}}.cshtml";
            }

            yield return "/Roles/Shared/Views/{1}/{0}.cshtml";
            yield return "/Roles/Shared/Views/{0}.cshtml";
            yield return "/Roles/Shared/Views/Shared/{0}.cshtml";

            foreach (var location in viewLocations)
            {
                yield return location;
            }
        }
    }
}
