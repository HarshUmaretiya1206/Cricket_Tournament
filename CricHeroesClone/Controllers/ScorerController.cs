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
        private readonly IBallByBallRepository _ballRepo;
        private readonly IPlayerRepository _playerRepo;

        public ScorerController(
            IMatchRepository matchRepo, 
            IScoreRepository scoreRepo, 
            ITeamRepository teamRepo,
            IBallByBallRepository ballRepo,
            IPlayerRepository playerRepo)
        {
            _matchRepo = matchRepo;
            _scoreRepo = scoreRepo;
            _teamRepo = teamRepo;
            _ballRepo = ballRepo;
            _playerRepo = playerRepo;
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

            // Redirect scorers to the public live index instead of a scorer-specific view
            return RedirectToAction("Index", "Live");
        }

        public async Task<IActionResult> ScoreMatch(int matchId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();

            var match = await _matchRepo.GetByIdAsync(matchId);
            if (match == null) return NotFound();

            var balls = await _ballRepo.GetBallsByMatchAsync(matchId);
            var players = await _playerRepo.GetPlayersByTeamAsync(match.TeamAId);
            players = players.Concat(await _playerRepo.GetPlayersByTeamAsync(match.TeamBId));

            ViewBag.Match = match;
            ViewBag.Balls = balls;
            ViewBag.Players = players;
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecordBall(BallByBall ball)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();

            try
            {
                // Validate ball data
                if (ball.MatchId <= 0 || ball.Innings <= 0 || ball.OverNumber <= 0 || ball.BallNumber <= 0)
                {
                    TempData["Error"] = "Invalid ball data";
                    return RedirectToAction("ScoreMatch", new { matchId = ball.MatchId });
                }

                // Check if this ball already exists
                var existingBall = await _ballRepo.GetBallByOverAndBallAsync(ball.MatchId, ball.Innings, ball.OverNumber, ball.BallNumber);
                if (existingBall != null)
                {
                    TempData["Error"] = $"Ball {ball.OverNumber}.{ball.BallNumber} already recorded";
                    return RedirectToAction("ScoreMatch", new { matchId = ball.MatchId });
                }

                // Add the ball
                await _ballRepo.AddAsync(ball);

                // Update match score
                await UpdateMatchScore(ball.MatchId, ball.Innings);

                TempData["Message"] = $"Ball {ball.OverNumber}.{ball.BallNumber} recorded successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error recording ball: {ex.Message}";
            }

            return RedirectToAction("ScoreMatch", new { matchId = ball.MatchId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBall(int ballId, BallByBall updatedBall)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();

            try
            {
                var existingBall = await _ballRepo.GetByIdAsync(ballId);
                if (existingBall == null) return NotFound();

                // Update ball properties
                existingBall.Runs = updatedBall.Runs;
                existingBall.IsWicket = updatedBall.IsWicket;
                existingBall.IsWide = updatedBall.IsWide;
                existingBall.IsNoBall = updatedBall.IsNoBall;
                existingBall.IsBye = updatedBall.IsBye;
                existingBall.IsLegBye = updatedBall.IsLegBye;
                existingBall.WicketType = updatedBall.WicketType;
                existingBall.WicketDescription = updatedBall.WicketDescription;
                existingBall.ExtraRuns = updatedBall.ExtraRuns;
                existingBall.ExtraType = updatedBall.ExtraType;
                existingBall.Commentary = updatedBall.Commentary;

                await _ballRepo.UpdateAsync(existingBall);

                // Update match score
                await UpdateMatchScore(existingBall.MatchId, existingBall.Innings);

                TempData["Message"] = "Ball updated successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating ball: {ex.Message}";
            }

            return RedirectToAction("ScoreMatch", new { matchId = updatedBall.MatchId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBall(int ballId, int matchId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();

            try
            {
                var ball = await _ballRepo.GetByIdAsync(ballId);
                if (ball == null) return NotFound();

                await _ballRepo.DeleteAsync(ballId);

                // Update match score
                await UpdateMatchScore(matchId, ball.Innings);

                TempData["Message"] = "Ball deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting ball: {ex.Message}";
            }

            return RedirectToAction("ScoreMatch", new { matchId = matchId });
        }

        [HttpPost]
        public async Task<IActionResult> StartInnings(int matchId, int innings, int battingTeamId, int bowlingTeamId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();

            try
            {
                var match = await _matchRepo.GetByIdAsync(matchId);
                if (match == null) return NotFound();

                // Update match with new innings
                match.CurrentInnings = innings;
                match.BattingTeamId = battingTeamId;
                match.BowlingTeamId = bowlingTeamId;
                match.CurrentRuns = 0;
                match.CurrentWickets = 0;
                match.CurrentOvers = 0;

                await _matchRepo.UpdateAsync(match);

                TempData["Message"] = $"Innings {innings} started successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error starting innings: {ex.Message}";
            }

            return RedirectToAction("ScoreMatch", new { matchId = matchId });
        }

        [HttpPost]
        public async Task<IActionResult> EndInnings(int matchId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Scorer") return Forbid();

            try
            {
                var match = await _matchRepo.GetByIdAsync(matchId);
                if (match == null) return NotFound();

                if (match.CurrentInnings == 1)
                {
                    // Set target for second innings
                    match.TargetRuns = match.CurrentRuns + 1;
                    match.CurrentInnings = 2;
                    match.BattingTeamId = match.TeamBId;
                    match.BowlingTeamId = match.TeamAId;
                    match.CurrentRuns = 0;
                    match.CurrentWickets = 0;
                    match.CurrentOvers = 0;
                }
                else
                {
                    // End of second innings - determine result
                    if (match.CurrentRuns >= match.TargetRuns)
                    {
                        match.Result = $"{match.TeamBName} won by {10 - match.CurrentWickets} wickets";
                        match.WinningTeamId = match.TeamBId;
                    }
                    else
                    {
                        match.Result = $"{match.TeamAName} won by {match.TargetRuns - match.CurrentRuns - 1} runs";
                        match.WinningTeamId = match.TeamAId;
                    }
                    match.Status = "Completed";
                }

                await _matchRepo.UpdateAsync(match);

                TempData["Message"] = match.CurrentInnings == 2 ? "First innings ended, second innings started" : "Match completed";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error ending innings: {ex.Message}";
            }

            return RedirectToAction("ScoreMatch", new { matchId = matchId });
        }

        private async Task UpdateMatchScore(int matchId, int innings)
        {
            try
            {
                var balls = await _ballRepo.GetBallsByInningsAsync(matchId, innings);
                var match = await _matchRepo.GetByIdAsync(matchId);

                if (match != null)
                {
                    int totalRuns = 0;
                    int totalWickets = 0;
                    int totalBalls = balls.Count();

                    foreach (var ball in balls)
                    {
                        totalRuns += ball.Runs;
                        if (ball.IsWicket) totalWickets++;
                    }

                    float overs = totalBalls / 6.0f + (totalBalls % 6) / 10.0f;

                    match.CurrentRuns = totalRuns;
                    match.CurrentWickets = totalWickets;
                    match.CurrentOvers = (float)Math.Round(overs, 1);
                    match.LastUpdated = DateTime.Now;

                    await _matchRepo.UpdateAsync(match);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the ball recording
                Console.WriteLine($"Error updating match score: {ex.Message}");
            }
        }
    }
}
