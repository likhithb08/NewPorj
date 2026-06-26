using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class KycController : Controller
    {
        public IActionResult Verify(int id) => View();
        public IActionResult History(int id) => View();
    }
}
