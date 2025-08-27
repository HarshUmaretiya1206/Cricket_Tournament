using Microsoft.AspNetCore.Mvc;     
using CricHeroesClone.Models;
using CricHeroesClone.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CricHeroesClone.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IPlayerRepository _repo;

        public PlayerController(IPlayerRepository playerRepository)
        {
            _repo = playerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var players = await _repo.GetAllAsync();
            return View(players);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Fetch teams for dropdown
            var teams = await _repo.GetTeamsAsync(); // You may need to add this method to your repository
            ViewBag.Teams = new SelectList(teams, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Player player)
        {
            if (ModelState.IsValid)
            {
                await _repo.AddAsync(player);
                return RedirectToAction("Index");
            }
            // Repopulate teams if model is invalid
            var teams = await _repo.GetTeamsAsync();
            ViewBag.Teams = new SelectList(teams, "Id", "Name");
            return View(player);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
