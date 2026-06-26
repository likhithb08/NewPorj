using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class DisbursementController : Controller
    {
        public IActionResult Create(int id) => View();
        public IActionResult History() => View();
    }
}
