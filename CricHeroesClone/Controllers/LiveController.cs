using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Repository;
using CricHeroesClone.Models;

namespace CricHeroesClone.Controllers
{
    public class LiveController : Controller
    {
        private readonly IMatchRepository _matchRepo;
        private readonly ITeamStandingRepository _standingRepo;
        private readonly IBallByBallRepository _ballRepo;
        private readonly ITournamentRepository _tournamentRepo;

        public LiveController(
            IMatchRepository matchRepo, 
            ITeamStandingRepository standingRepo,
            IBallByBallRepository ballRepo,
            ITournamentRepository tournamentRepo)
        {
            _matchRepo = matchRepo;
            _standingRepo = standingRepo;
            _ballRepo = ballRepo;
            _tournamentRepo = tournamentRepo;
        }

        public async Task<IActionResult> Index()
        {
            var liveMatches = await _matchRepo.GetLiveMatchesAsync();
            var upcomingMatches = await _matchRepo.GetUpcomingMatchesAsync();
            
            ViewBag.LiveMatches = liveMatches;
            ViewBag.UpcomingMatches = upcomingMatches;
            
            return View();
        }

        public async Task<IActionResult> MatchDetails(int matchId)
        {
            var match = await _matchRepo.GetByIdAsync(matchId);
            if (match == null) return NotFound();

            var balls = await _ballRepo.GetBallsByMatchAsync(matchId);
            var lastBall = await _ballRepo.GetLastBallAsync(matchId);
            
            ViewBag.Match = match;
            ViewBag.Balls = balls;
            ViewBag.LastBall = lastBall;
            
            return View();
        }

        public async Task<IActionResult> Viewer(int matchId)
        {
            var scores = await _matchRepo.GetScoreAsync(matchId);
            var match = await _matchRepo.GetByIdAsync(matchId);
            var balls = await _ballRepo.GetBallsByMatchAsync(matchId);
            
            if (scores == null)
            {
                ViewBag.ScoreA = "0/0";
                ViewBag.ScoreB = "0/0";
            }
            else
            {
                ViewBag.ScoreA = scores.ScoreA;
                ViewBag.ScoreB = scores.ScoreB;
            }
            
            ViewBag.MatchId = matchId;
            ViewBag.Match = match;
            ViewBag.Balls = balls;
            
            return View("LiveMatch");
        }

        public async Task<IActionResult> TeamStandings(int? tournamentId = null)
        {
            IEnumerable<TeamStanding> standings;
            
            if (tournamentId.HasValue)
            {
                standings = await _standingRepo.GetStandingsByTournamentAsync(tournamentId.Value);
                var tournament = await _tournamentRepo.GetByIdAsync(tournamentId.Value);
                ViewBag.Tournament = tournament;
            }
            else
            {
                standings = await _standingRepo.GetAllStandingsAsync();
            }
            
            // Sort by points (descending) then by net run rate (descending)
            standings = standings.OrderByDescending(s => s.Points)
                               .ThenByDescending(s => s.NetRunRate);
            
            return View(standings);
        }

        [HttpGet]
        public async Task<IActionResult> LiveScore(int matchId)
        {
            var match = await _matchRepo.GetByIdAsync(matchId);
            if (match == null) return NotFound();

            var balls = await _ballRepo.GetBallsByMatchAsync(matchId);
            var lastBall = await _ballRepo.GetLastBallAsync(matchId);
            
            var liveData = new
            {
                MatchId = match.Id,
                TeamAName = match.TeamAName,
                TeamBName = match.TeamBName,
                CurrentInnings = match.CurrentInnings,
                BattingTeam = match.CurrentInnings == 1 ? match.TeamAName : match.TeamBName,
                BowlingTeam = match.CurrentInnings == 1 ? match.TeamBName : match.TeamAName,
                CurrentRuns = match.CurrentRuns,
                CurrentWickets = match.CurrentWickets,
                CurrentOvers = match.CurrentOvers,
                TargetRuns = match.TargetRuns,
                LastBall = lastBall?.BallSummary ?? "No balls yet",
                LastUpdated = match.LastUpdated
            };
            
            return Json(liveData);
        }
    }
}
