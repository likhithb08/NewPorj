using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Create() => View();
        public IActionResult Edit(int id) => View();
        public IActionResult Details(int id) => View();
    }
}
