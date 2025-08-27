using Microsoft.AspNetCore.Mvc;

namespace CricHeroesClone.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();
            return View();
        }
    }
}
