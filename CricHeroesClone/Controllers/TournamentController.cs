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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tournament = await _tournamentRepository.GetByIdAsync(id);
            if (tournament == null)
                return NotFound();
            return View(tournament);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                // For now, we'll just redirect since UpdateAsync is not implemented
                // TODO: Implement UpdateAsync in TournamentRepository
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
