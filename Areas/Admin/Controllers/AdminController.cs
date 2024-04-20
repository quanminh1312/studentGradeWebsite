using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doancoso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Classes()
        {
            return RedirectToAction("Index", "Classes");
        }
        public IActionResult Students()
        {
            return RedirectToAction("Index", "Students");
        }
        public IActionResult Teachers()
        {
            return RedirectToAction("Index", "Teachers");
        }
        public IActionResult Grades()
        {
            return RedirectToAction("Index", "Grades");
        }
        public IActionResult Majors()
        {
            return RedirectToAction("Index", "Majors");
        }
        public IActionResult Semesters()
        {
            return RedirectToAction("Index", "Semesters");
        }
    }
}
