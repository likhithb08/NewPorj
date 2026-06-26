using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class CreditController : Controller
    {
        public IActionResult Evaluate(int id) => View();
        public IActionResult Details(int id) => View();
    }
}
