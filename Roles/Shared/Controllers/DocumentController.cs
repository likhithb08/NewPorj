using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Upload(int id) => View();
        public IActionResult Validate(int id) => View();
    }
}
