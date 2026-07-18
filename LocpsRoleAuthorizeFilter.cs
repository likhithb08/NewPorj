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
                role = user.FindFirst(RoleConstants.RoleClaimType)?.Value ?? "customer";
            }
            else
            {
                // Fallback to cookie for demo mode
                role = context.HttpContext.Request.Cookies[AuthConstants.RoleDisplayCookieName] ?? "customer";
            }

            // Role allowed routing prefixes - using RoleConstants tokens
            var permissions = new Dictionary<string, string[]>
            {
                { RoleConstants.CustomerToken, new[] { 
                    "/dashboard", 
                    "/loan/create", 
                    "/document/upload", 
                    "/notification", 
                    "/settings" 
                } },
                { RoleConstants.LoanOfficerToken, new[] { 
                    "/dashboard", 
                    "/customer", 
                    "/loan", 
                    "/kyc/verify", 
                    "/credit/evaluate", 
                    "/document/validate", 
                    "/document/upload", 
                    "/notification",
                    "/settings" 
                } },
                { RoleConstants.UnderWriterToken, new[] { 
                    "/dashboard", 
                    "/approval", 
                    "/disbursement/create", 
                    "/disbursement/history", 
                    "/notification",
                    "/settings" 
                } },
                { RoleConstants.AdminToken, new[] { 
                    "/dashboard", 
                    "/product", 
                    "/usermanagement", 
                    "/settings", 
                    "/disbursement/history", 
                    "/reports", 
                    "/notification" 
                } }
            };

            // Specific path blocks for roles - using RoleConstants tokens
            // 1. Admin cannot apply for loans or upload documents
            if (role == RoleConstants.AdminToken && (path.StartsWith("/loan/create") || path.StartsWith("/document/upload")))
            {
                RedirectToAccessDenied(context, role);
                return;
            }

            // 2. Customer cannot verify, score, or disburse
            if (role == RoleConstants.CustomerToken && (
                path.StartsWith("/customer") ||
                path.StartsWith("/kyc") ||
                path.StartsWith("/credit") ||
                path.StartsWith("/document/validate") ||
                path.StartsWith("/approval") ||
                path.StartsWith("/disbursement") ||
                path.StartsWith("/product") ||
                path.StartsWith("/usermanagement") ||
                path.StartsWith("/reports")
            ))
            {
                RedirectToAccessDenied(context, role);
                return;
            }

            // 3. Loan Officer must not register customers, apply for loans, approve, reject, or disburse
            if (role == RoleConstants.LoanOfficerToken && (
                path.StartsWith("/customer/create") ||
                path.StartsWith("/loan/create") ||
                path.StartsWith("/approval") ||
                path.StartsWith("/disbursement")
            ))
            {
                RedirectToAccessDenied(context, role);
                return;
            }

            // 4. Underwriter must not verify documents, calculate credit scores, register customers, apply for loans
            if (role == RoleConstants.UnderWriterToken && (
                path.StartsWith("/customer") ||
                path.StartsWith("/loan/create") ||
                path.StartsWith("/kyc") ||
                path.StartsWith("/credit") ||
                path.StartsWith("/document/validate") ||
                path.StartsWith("/document/upload")
            ))
            {
                RedirectToAccessDenied(context, role);
                return;
            }

            // 5. System configuration check (scoringrules and auditlogs only for admin)
            if (role != RoleConstants.AdminToken && (
                path.StartsWith("/settings/scoringrules") ||
                path.StartsWith("/settings/auditlogs")
            ))
            {
                RedirectToAccessDenied(context, role);
                return;
            }

            if (!permissions.TryGetValue(role, out var allowedPaths))
            {
                RedirectToAccessDenied(context, role);
                return;
            }

            bool isAuthorized = false;
            foreach (var allowed in allowedPaths)
            {
                if (path == allowed || path.StartsWith(allowed + "/"))
                {
                    isAuthorized = true;
                    break;
                }
            }

            if (!isAuthorized)
            {
                RedirectToAccessDenied(context, role);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private void RedirectToAccessDenied(ActionExecutingContext context, string role)
        {
            context.Result = new ViewResult
            {
                ViewName = "AccessDenied",
                StatusCode = 403
            };
        }
    }
}
