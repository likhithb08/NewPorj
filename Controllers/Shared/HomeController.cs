using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
