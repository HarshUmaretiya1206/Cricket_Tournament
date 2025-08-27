using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CricHeroesClone.Models;
using CricHeroesClone.Repository;

namespace CricHeroesClone.Controllers
{
    public class TeamController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITournamentRepository _tournamentRepository;

        public TeamController(ITeamRepository teamRepository, ITournamentRepository tournamentRepository)
        {
            _teamRepository = teamRepository;
            _tournamentRepository = tournamentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _teamRepository.GetAllAsync();
            return View(teams);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var tournaments = await _tournamentRepository.GetAllAsync();
            ViewBag.Tournaments = new SelectList(tournaments, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Team team)
        {
            if (ModelState.IsValid)
            {
                await _teamRepository.AddAsync(team);
                return RedirectToAction("Index");
            }
            return View(team);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _teamRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
