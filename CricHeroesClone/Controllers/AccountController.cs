using CricHeroesClone.Data;
using CricHeroesClone.Repository;
using CricHeroesClone.Models; // Add this line to reference the User model
using Microsoft.AspNetCore.Mvc;

namespace CricHeroesClone.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        public AccountController(IUserRepository userRepo) => _userRepo = userRepo;

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userRepo.LoginAsync(username, password);
            if (user == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserID", user.Id);

            // redirect based on role
            return user.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Scorer" => RedirectToAction("Dashboard", "Scorer"),
                "Captain" => RedirectToAction("Dashboard", "Captain"),
                "Viewer" => RedirectToAction("Dashboard", "Viewer"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            // Set default role to Viewer for new registrations
            if (string.IsNullOrEmpty(model.Role))
            {
                model.Role = "Viewer";
            }
            
            await _userRepo.RegisterAsync(model);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
