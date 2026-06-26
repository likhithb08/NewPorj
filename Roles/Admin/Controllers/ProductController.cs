using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Create() => View();
        public IActionResult Details(int id) => View();
    }
}
