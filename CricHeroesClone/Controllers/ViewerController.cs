using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Repository;
using CricHeroesClone.Models;

namespace CricHeroesClone.Controllers
{
    public class ViewerController : Controller
    {
        private readonly IMatchRepository _matchRepo;
        private readonly ITeamRepository _teamRepo;
        private readonly ITournamentRepository _tournamentRepo;

        public ViewerController(IMatchRepository matchRepo, ITeamRepository teamRepo, ITournamentRepository tournamentRepo)
        {
            _matchRepo = matchRepo;
            _teamRepo = teamRepo;
            _tournamentRepo = tournamentRepo;
        }

        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Viewer") return Forbid();
            
            var liveMatches = await _matchRepo.GetLiveMatchesAsync();
            var upcomingMatches = await _matchRepo.GetUpcomingMatchesAsync();
            var teams = await _teamRepo.GetAllAsync();
            var tournaments = await _tournamentRepo.GetAllAsync();
            
            ViewBag.LiveMatches = liveMatches;
            ViewBag.UpcomingMatches = upcomingMatches;
            ViewBag.Teams = teams;
            ViewBag.Tournaments = tournaments;
            
            return View();
        }

        public async Task<IActionResult> LiveMatches()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Viewer") return Forbid();
            
            var matches = await _matchRepo.GetLiveMatchesAsync();
            return View(matches);
        }

        public async Task<IActionResult> MatchDetails(int matchId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Viewer") return Forbid();
            
            var match = await _matchRepo.GetByIdAsync(matchId);
            if (match == null) return NotFound();
            
            return View(match);
        }

        public async Task<IActionResult> TeamStandings()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Viewer") return Forbid();
            
            var teams = await _teamRepo.GetAllAsync();
            return View(teams);
        }
    }
}
