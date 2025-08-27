using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllAsync();
        Task AddAsync(Player player);
        Task DeleteAsync(int playerId);

        // Add this method
        Task<IEnumerable<Team>> GetTeamsAsync();
    }
}
