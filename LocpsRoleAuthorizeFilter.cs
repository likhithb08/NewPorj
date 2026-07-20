using LOCPS.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace LOCPS
{
    public class LocpsRoleAuthorizeFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var path = context.HttpContext.Request.Path.Value?.ToLower() ?? "";

            // Skip check for login page, register page, logout, public landing page, and static resources
            if (path == "/" ||
                path.StartsWith("/account/login") ||
                path.StartsWith("/account/register") ||
                path.StartsWith("/account/logout") ||
                path.StartsWith("/account/accessdenied") ||
                path.StartsWith("/home") ||
                path.Contains("favicon") ||
                path.StartsWith("/css/") ||
                path.StartsWith("/js/") ||
                path.StartsWith("/lib/"))
            {
                return;
            }

            // Get role from claims first, fall back to cookie for legacy support
            var user = context.HttpContext.User;
            string role = "customer"; // default

            if (user.Identity?.IsAuthenticated == true)
            {
                role = user.FindFirst(RoleConstants.RoleClaimType)?.Value?.ToLowerInvariant() ?? "customer";
            }
            else
            {
                // Fallback to cookie for demo mode
                role = context.HttpContext.Request.Cookies[AuthConstants.RoleDisplayCookieName]?.ToLowerInvariant() ?? "customer";
            }

            // ─── Role-to-allowed-paths map ───────────────────────────────────────────
            var permissions = new Dictionary<string, string[]>
            {
                { RoleConstants.CustomerToken, new[] {
                    "/dashboard",
                    "/customer",          // CustomerController (index, create, details, submitkyc)
                    "/notification",
                    "/settings"
                } },
                { RoleConstants.LoanOfficerToken, new[] {
                    "/dashboard",
                    "/customer",
                    "/loan",
                    "/kyc",
                    "/credit",
                    "/document",
                    "/notification",
                    "/settings"
                } },
                { RoleConstants.UnderWriterToken, new[] {
                    "/dashboard",
                    "/approval",
                    "/disbursement",
                    "/notification",
                    "/settings"
                } },
                { RoleConstants.AdminToken, new[] {
                    "/dashboard",
                    "/product",
                    "/usermanagement",
                    "/settings",
                    "/disbursement",
                    "/reports",
                    "/notification"
                } }
            };

            // ─── Fine-grained DENY rules ─────────────────────────────────────────────

            // Admin: cannot apply for loans or upload documents
            if (role == RoleConstants.AdminToken &&
                (path.StartsWith("/customer/create") || path.StartsWith("/document/upload")))
            {
                RedirectToAccessDenied(context);
                return;
            }

            // Customer: cannot access staff-only sections
            if (role == RoleConstants.CustomerToken && (
                path.StartsWith("/kyc") ||
                path.StartsWith("/credit") ||
                path.StartsWith("/document/validate") ||
                path.StartsWith("/approval") ||
                path.StartsWith("/disbursement") ||
                path.StartsWith("/product") ||
                path.StartsWith("/usermanagement") ||
                path.StartsWith("/reports") ||
                path.StartsWith("/loan") // Loan officer loan-management routes (not customer routes)
            ))
            {
                RedirectToAccessDenied(context);
                return;
            }

            // Loan Officer: cannot approve/disburse
            if (role == RoleConstants.LoanOfficerToken && (
                path.StartsWith("/approval") ||
                path.StartsWith("/disbursement") ||
                path.StartsWith("/usermanagement") ||
                path.StartsWith("/product") ||
                path.StartsWith("/reports")
            ))
            {
                RedirectToAccessDenied(context);
                return;
            }

            // Underwriter: cannot do KYC, credit, documents, or customer registration
            if (role == RoleConstants.UnderWriterToken && (
                path.StartsWith("/customer") ||
                path.StartsWith("/loan") ||
                path.StartsWith("/kyc") ||
                path.StartsWith("/credit") ||
                path.StartsWith("/document") ||
                path.StartsWith("/usermanagement") ||
                path.StartsWith("/product") ||
                path.StartsWith("/reports")
            ))
            {
                RedirectToAccessDenied(context);
                return;
            }

            // Scoring rules and audit logs: admin only
            if (role != RoleConstants.AdminToken && (
                path.StartsWith("/settings/scoringrules") ||
                path.StartsWith("/settings/auditlogs")
            ))
            {
                RedirectToAccessDenied(context);
                return;
            }

            // ─── Final whitelist check ───────────────────────────────────────────────
            if (!permissions.TryGetValue(role, out var allowedPaths))
            {
                RedirectToAccessDenied(context);
                return;
            }

            bool isAuthorized = false;
            foreach (var allowed in allowedPaths)
            {
                if (path == allowed || path.StartsWith(allowed + "/") || path.StartsWith(allowed + "?"))
                {
                    isAuthorized = true;
                    break;
                }
            }

            if (!isAuthorized)
            {
                RedirectToAccessDenied(context);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        private static void RedirectToAccessDenied(ActionExecutingContext context)
        {
            context.Result = new ViewResult
            {
                ViewName = "AccessDenied",
                StatusCode = 403
            };
        }
    }
}
