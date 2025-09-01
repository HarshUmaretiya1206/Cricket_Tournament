using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Repository;
using CricHeroesClone.Models;

namespace CricHeroesClone.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly ITeamRepository _teamRepo;
        private readonly IPlayerRepository _playerRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly ITournamentRepository _tournamentRepo;

        public AdminController(
            IUserRepository userRepo, 
            ITeamRepository teamRepo, 
            IPlayerRepository playerRepo, 
            IMatchRepository matchRepo, 
            ITournamentRepository tournamentRepo)
        {
            _userRepo = userRepo;
            _teamRepo = teamRepo;
            _playerRepo = playerRepo;
            _matchRepo = matchRepo;
            _tournamentRepo = tournamentRepo;
        }

        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();
            
            // Get system overview data
            var totalUsers = await _userRepo.GetTotalUsersAsync();
            var totalTeams = await _teamRepo.GetTotalTeamsAsync();
            var totalPlayers = await _playerRepo.GetTotalPlayersAsync();
            var totalMatches = await _matchRepo.GetTotalMatchesAsync();
            var totalTournaments = await _tournamentRepo.GetTotalTournamentsAsync();
            
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalTeams = totalTeams;
            ViewBag.TotalPlayers = totalPlayers;
            ViewBag.TotalMatches = totalMatches;
            ViewBag.TotalTournaments = totalTournaments;
            
            return View();
        }

        public async Task<IActionResult> UserManagement()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();
            
            var users = await _userRepo.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> TeamManagement()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();
            
            var teams = await _teamRepo.GetAllAsync();
            return View(teams);
        }

        public async Task<IActionResult> TournamentManagement()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();
            
            var tournaments = await _tournamentRepo.GetAllAsync();
            return View(tournaments);
        }

        public async Task<IActionResult> SystemSettings()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(int userId, string newRole)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();
            
            await _userRepo.UpdateUserRoleAsync(userId, newRole);
            return RedirectToAction("UserManagement");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();
            
            await _userRepo.DeleteAsync(userId);
            return RedirectToAction("UserManagement");
        }
    }
}
