using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login() => View();
        
        public IActionResult Register() => View();
        
        public IActionResult Logout()
        {
            // Simple mock redirect to login screen
            return RedirectToAction("Login");
        }
    }
}
