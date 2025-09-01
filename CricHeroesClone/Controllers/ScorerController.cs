using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Repository;
using CricHeroesClone.Models;

namespace CricHeroesClone.Controllers
{
    public class ScorerController : Controller
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IScoreRepository _scoreRepo;
        private readonly ITeamRepository _teamRepo;

        public ScorerController(IMatchRepository matchRepo, IScoreRepository scoreRepo, ITeamRepository teamRepo)
        {
            _matchRepo = matchRepo;
            _scoreRepo = scoreRepo;
            _teamRepo = teamRepo;
        }

        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();
            
            var userId = HttpContext.Session.GetInt32("UserID");
            ViewBag.UserId = userId;
            return View();
        }

        public async Task<IActionResult> LiveMatches()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();

            var matches = await _matchRepo.GetLiveMatchesAsync();
            return View(matches);
        }

        public async Task<IActionResult> ScoreMatch(int matchId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();

            var match = await _matchRepo.GetByIdAsync(matchId);
            if (match == null) return NotFound();

            ViewBag.Match = match;
            return View();
        }
    }
}
