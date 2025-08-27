using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Models;
using CricHeroesClone.Repository;

namespace CricHeroesClone.Controllers
{
    public class MatchController : Controller
    {
        private readonly IMatchRepository _matchRepository;

        public MatchController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<IActionResult> Index()
        {
            var matches = await _matchRepository.GetAllAsync();
            return View(matches);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Match match)
        {
            if (ModelState.IsValid)
            {
                await _matchRepository.AddAsync(match);
                return RedirectToAction("Index");
            }
            return View(match);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _matchRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

    }
}
