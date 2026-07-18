using LOCPS.Constants;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOCPS.Controllers
{
    public class AccountController : Controller
    {
        public readonly IUserServices _userServices;
        public AccountController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var user = await _userServices.LoginAsync(email, password);

                // Create claims for the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(RoleConstants.RoleClaimType, RoleConstants.GetRoleToken(user.RoleId)),
                    new Claim(RoleConstants.RoleIdClaimType, user.RoleId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(AuthConstants.SessionHours)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Set role display cookie for view location expander (legacy support)
                Response.Cookies.Append(AuthConstants.RoleDisplayCookieName, RoleConstants.GetRoleToken(user.RoleId), new CookieOptions
                {
                    Path = "/",
                    MaxAge = TimeSpan.FromDays(AuthConstants.SessionHours),
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                });

                // Redirect to role-specific dashboard
                return RedirectToRoleBasedDashboard(user.RoleId);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid) return View(user);
            await _userServices.RegisterUserAsync(user);

            return RedirectToAction("Login");
        } 

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete(AuthConstants.RoleDisplayCookieName);
            return RedirectToAction("Login");
        }

        private void SetRoleCookie(int roleId)
        {
            // Use RoleConstants for single source of truth
            string roleToken = RoleConstants.GetRoleToken(roleId);

            Response.Cookies.Append(AuthConstants.RoleDisplayCookieName, roleToken, new CookieOptions
            {
                Path = "/",
                MaxAge = TimeSpan.FromHours(AuthConstants.SessionHours),
                HttpOnly = false,
                SameSite = SameSiteMode.Lax
            });
        }

        private IActionResult RedirectToRoleBasedDashboard(int roleId)
        {
            // Redirect based on RoleId
            // The RoleBasedViewLocationExpander will use the cookie to find the correct view
            // RoleId: 1=Customer, 2=Admin, 3=LoanOfficer, 4=UnderWriter
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
