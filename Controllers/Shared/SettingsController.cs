using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult ScoringRules() => View();
        public IActionResult AuditLogs() => View();
    }
}
