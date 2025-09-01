using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Repository;
using CricHeroesClone.Models;

namespace CricHeroesClone.Controllers
{
    public class CaptainController : Controller
    {
        private readonly ITeamRepository _teamRepo;
        private readonly IPlayerRepository _playerRepo;
        private readonly IMatchRepository _matchRepo;

        public CaptainController(ITeamRepository teamRepo, IPlayerRepository playerRepo, IMatchRepository matchRepo)
        {
            _teamRepo = teamRepo;
            _playerRepo = playerRepo;
            _matchRepo = matchRepo;
        }

        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Captain") return Forbid();
            
            var userId = HttpContext.Session.GetInt32("UserID");
            var team = await _teamRepo.GetTeamByCaptainAsync(userId.Value);
            
            if (team == null) return NotFound();
            
            ViewBag.Team = team;
            var players = await _playerRepo.GetPlayersByTeamAsync(team.Id);
            ViewBag.Players = players;
            
            return View();
        }

        public async Task<IActionResult> TeamPlayers()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Captain") return Forbid();
            
            var userId = HttpContext.Session.GetInt32("UserID");
            var team = await _teamRepo.GetTeamByCaptainAsync(userId.Value);
            
            if (team == null) return NotFound();
            
            var players = await _playerRepo.GetPlayersByTeamAsync(team.Id);
            return View(players);
        }

        public async Task<IActionResult> UpcomingMatches()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Captain") return Forbid();
            
            var userId = HttpContext.Session.GetInt32("UserID");
            var team = await _teamRepo.GetTeamByCaptainAsync(userId.Value);
            
            if (team == null) return NotFound();
            
            var matches = await _matchRepo.GetUpcomingMatchesByTeamAsync(team.Id);
            return View(matches);
        }
    }
}
