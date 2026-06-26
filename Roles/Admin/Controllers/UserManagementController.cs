using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class UserManagementController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Create() => View();
        public IActionResult Edit(int id) => View();
    }
}
