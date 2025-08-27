using Microsoft.AspNetCore.Mvc;
using CricHeroesClone.Models;
using CricHeroesClone.Repository;

namespace CricHeroesClone.Controllers
{
    public class TournamentController : Controller
    {
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentController(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var tournaments = await _tournamentRepository.GetAllAsync();
            return View(tournaments);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                await _tournamentRepository.AddAsync(tournament);
                return RedirectToAction("Index");
            }
            return View(tournament);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _tournamentRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
