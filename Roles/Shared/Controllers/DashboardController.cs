using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index() => View();
    }
}
