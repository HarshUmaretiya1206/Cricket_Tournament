using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Repository;

namespace CricHeroesClone.Controllers
{
    public class LiveController : Controller
    {
        private readonly IMatchRepository _matchRepo;
        public LiveController(IMatchRepository matchRepo) => _matchRepo = matchRepo;

        public async Task<IActionResult> Index()
        {
            var list = await _matchRepo.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Viewer(int matchId)
        {
            var scores = await _matchRepo.GetScoreAsync(matchId);
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
            return View();
        }

    }
}
