using CricHeroesClone.Data;
using CricHeroesClone.Repository;
using CricHeroesClone.Models;
using Microsoft.AspNetCore.Mvc;

namespace CricHeroesClone.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly ITeamRepository _teamRepo;
        
        public AccountController(IUserRepository userRepo, ITeamRepository teamRepo)
        {
            _userRepo = userRepo;
            _teamRepo = teamRepo;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Username and password are required";
                    return View();
                }

                var user = await _userRepo.LoginAsync(username, password);
                if (user == null)
                {
                    ViewBag.Error = "Invalid username or password";
                    return View();
                }

                // Store user information in session
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetInt32("UserID", user.Id);

                // Special handling for Captain role - verify team association
                if (user.Role == "Captain")
                {
                    try
                    {
                        var team = await _teamRepo.GetTeamByCaptainAsync(user.Id);
                        if (team == null)
                        {

                            // Allow login but warn and proceed without team bindings
                            TempData["Warning"] = "Captain account has no team assigned yet.";
                        }
                        else
                        {
                            // Store team information in session for captain
                            HttpContext.Session.SetInt32("TeamId", team.Id);
                            HttpContext.Session.SetString("TeamName", team.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Allow login with a warning instead of blocking
                        TempData["Warning"] = "Could not verify captain's team at the moment.";
                        ViewBag.DebugInfo = $"Error: {ex.Message}\nUser ID: {user.Id}, Role: {user.Role}";
                    }
                }

                // Log successful login
                Console.WriteLine($"User {username} logged in successfully with role {user.Role}");

                // Redirect based on role
                return user.Role switch
                {
                    "Admin" => RedirectToAction("Dashboard", "Admin"),
                    "Scorer" => RedirectToAction("Dashboard", "Scorer"),
                    // Redirect captains to Live index for now (no dedicated dashboard)
                    "Captain" => RedirectToAction("Index", "Live"),
                    "Viewer" => RedirectToAction("Dashboard", "Viewer"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred during login. Please try again.";
                ViewBag.DebugInfo = $"Exception: {ex.Message}\nStackTrace: {ex.StackTrace}";
                Console.WriteLine($"Login error: {ex.Message}");
                return View();
            }
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Set default role to Viewer for new registrations
                    if (string.IsNullOrEmpty(model.Role))
                    {
                        model.Role = "Viewer";
                    }
                    
                    await _userRepo.RegisterAsync(model);
                    ViewBag.Success = "Registration successful! Please login with your credentials.";
                    return View("Login");
                }
                else
                {
                    ViewBag.Error = "Please correct the errors below.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred during registration. Please try again.";
                ViewBag.DebugInfo = $"Exception: {ex.Message}";
                Console.WriteLine($"Registration error: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            try
            {
                var username = HttpContext.Session.GetString("Username");
                var role = HttpContext.Session.GetString("UserRole");
                
                // Log logout
                if (!string.IsNullOrEmpty(username))
                {
                    Console.WriteLine($"User {username} ({role}) logged out");
                }
                
                HttpContext.Session.Clear();
                ViewBag.Success = "You have been logged out successfully.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
        }

        // Helper method to check if user is logged in
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
        }

        // Helper method to get current user role
        private string? GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole");
        }
    }
}
