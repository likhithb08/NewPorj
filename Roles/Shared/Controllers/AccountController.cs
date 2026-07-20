using LOCPS.Constants;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using LOCPS.ViewModels.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOCPS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var user = await _userService.LoginAsync(email, password);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user!.UserId.ToString()),
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

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Set role display cookie for view location expander
                Response.Cookies.Append(AuthConstants.RoleDisplayCookieName, RoleConstants.GetRoleToken(user.RoleId), new CookieOptions
                {
                    Path = "/",
                    MaxAge = TimeSpan.FromDays(AuthConstants.SessionHours),
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                });

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _userService.RegisterUserAsync(model);
                TempData["Success"] = "Registration successful. Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete(AuthConstants.RoleDisplayCookieName);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
