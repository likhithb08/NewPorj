using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class ApprovalController : Controller
    {
        public IActionResult Review(int id) => View();
        public IActionResult Approve(int id) => View();
        public IActionResult Reject(int id) => View();
    }
}
