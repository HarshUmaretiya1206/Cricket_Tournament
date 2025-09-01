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
            var matches = await _matchRepo.GetAllAsync();
            return View(matches); // Pass matches to view
        }

        // Show Score page for a specific match
        public async Task<IActionResult> Score(int matchId)
        {
            if (matchId <= 0)
                return BadRequest("Invalid matchId.");

            var match = await _matchRepo.GetByIdAsync(matchId);
            if (match == null)
                return NotFound("Match not found.");

            ViewBag.MatchId = matchId;
            return View(); // Views/Score/Score.cshtml
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

        // Handle RecordBall form submission
        [HttpPost]
        public async Task<IActionResult> RecordBall(int matchId, int batsmanId, int bowlerId, int runs, bool isWicket)
        {
            // Map to Score object (basic example)
            var score = new Score
            {
                MatchId = matchId,
                TeamId = 1, // you can adjust this to actual teamId logic
                Runs = runs,
                Wickets = isWicket ? 1 : 0,
                Overs = 0 // overs tracking logic needed
            };

            await _scoreRepo.UpdateScoreAsync(score);

            TempData["Message"] = "Ball recorded successfully!";
            return RedirectToAction("Score", new { matchId });
        }
    }
}
