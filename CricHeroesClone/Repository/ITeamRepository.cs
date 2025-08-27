using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task AddAsync(Team team);
        Task DeleteAsync(int teamId);
    }
}
