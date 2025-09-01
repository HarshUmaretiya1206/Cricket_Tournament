using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Models;
using CricHeroesClone.Repository;

namespace CricHeroesClone.Controllers
{
    public class ScoreController : Controller
    {
        private readonly IScoreRepository _scoreRepo;
        private readonly IMatchRepository _matchRepo;

        public ScoreController(IScoreRepository scoreRepo, IMatchRepository matchRepo)
        {
            _scoreRepo = scoreRepo;
            _matchRepo = matchRepo;
        }

        // Live score page with match selection
        public async Task<IActionResult> Live()
        {
            try
            {
                var matches = await _matchRepo.GetAllAsync();
                return View(matches ?? new List<Match>());
            }
            catch (Exception ex)
            {
                // Log the exception in production
                ViewBag.Error = "Unable to load matches. Please try again later.";
                return View(new List<Match>());
            }
        }

        // Get scores JSON for selected match
        [HttpGet]
        public async Task<IActionResult> GetScores(int matchId)
        {
            if (matchId <= 0) return BadRequest("Invalid matchId.");
            var scores = await _scoreRepo.GetScoresByMatchAsync(matchId);
            return Json(scores);
        }

        // Update score endpoint (used by form/ajax)
        [HttpPost]
        public async Task<IActionResult> UpdateScore([FromBody] Score score)
        {
            if (score.MatchId <= 0 || score.TeamId <= 0)
                return BadRequest("Invalid MatchId or TeamId.");

            await _scoreRepo.UpdateScoreAsync(score);
            return Ok();
        }
    }
}
