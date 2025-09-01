using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task AddAsync(Team team);
        Task DeleteAsync(int teamId);
        Task<int> GetTotalTeamsAsync();
        Task<Team?> GetTeamByCaptainAsync(int captainId);
        Task<Team?> GetByIdAsync(int teamId);
        Task UpdateCaptainAsync(int teamId, int? captainId);
    }
}
