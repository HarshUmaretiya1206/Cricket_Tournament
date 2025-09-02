using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllAsync();
        Task AddAsync(Player player);
        Task UpdateAsync(Player player);
        Task DeleteAsync(int playerId);

       
        Task<IEnumerable<Team>> GetTeamsAsync();
        Task<int> GetTotalPlayersAsync();
        Task<IEnumerable<Player>> GetPlayersByTeamAsync(int teamId);
        Task<Player?> GetByIdAsync(int playerId);
    }
}
