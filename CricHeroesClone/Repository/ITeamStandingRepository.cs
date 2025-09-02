using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface ITeamStandingRepository
    {
        Task<IEnumerable<TeamStanding>> GetStandingsByTournamentAsync(int tournamentId);
        Task<TeamStanding?> GetStandingByTeamAndTournamentAsync(int teamId, int tournamentId);
        Task AddAsync(TeamStanding standing);
        Task UpdateAsync(TeamStanding standing);
        Task DeleteAsync(int id);
        Task<IEnumerable<TeamStanding>> GetAllStandingsAsync();
        Task UpdateStandingsAfterMatchAsync(int matchId);
    }
}
