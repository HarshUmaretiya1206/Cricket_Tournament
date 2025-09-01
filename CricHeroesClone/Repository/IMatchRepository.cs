using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetAllAsync();
        Task<Match?> GetByIdAsync(int id);
        Task AddAsync(Match match);
        Task UpdateAsync(Match match);
        Task DeleteAsync(int id);

        // Add this ↓
        Task<MatchScoreDto?> GetScoreAsync(int matchId);
        Task<int> GetTotalMatchesAsync();
        Task<IEnumerable<Match>> GetLiveMatchesAsync();
        Task<IEnumerable<Match>> GetUpcomingMatchesAsync();
        Task<IEnumerable<Match>> GetUpcomingMatchesByTeamAsync(int teamId);
    }
}
