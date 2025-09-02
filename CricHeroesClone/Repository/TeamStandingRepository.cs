using CricHeroesClone.Models;
using CricHeroesClone.Data;
using Dapper;
using System.Data;

namespace CricHeroesClone.Repository
{
    public class TeamStandingRepository : ITeamStandingRepository
    {
        private readonly DapperContext _context;

        public TeamStandingRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeamStanding>> GetStandingsByTournamentAsync(int tournamentId)
        {
            using var connection = _context.CreateConnection();
            var standings = await connection.QueryAsync<TeamStanding>(
                "spGetTeamStandings",
                new { TournamentId = tournamentId },
                commandType: CommandType.StoredProcedure);
            return standings;
        }

        public async Task<TeamStanding?> GetStandingByTeamAndTournamentAsync(int teamId, int tournamentId)
        {
            var standings = await GetStandingsByTournamentAsync(tournamentId);
            return standings.FirstOrDefault(s => s.TeamId == teamId);
        }

        public async Task AddAsync(TeamStanding standing)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                @"INSERT INTO TeamStandings (TeamId, TournamentId, MatchesPlayed, MatchesWon, 
                    MatchesLost, MatchesTied, Points, NetRunRate) 
                VALUES (@TeamId, @TournamentId, @MatchesPlayed, @MatchesWon, 
                    @MatchesLost, @MatchesTied, @Points, @NetRunRate)",
                standing);
        }

        public async Task UpdateAsync(TeamStanding standing)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                @"UPDATE TeamStandings SET 
                    MatchesPlayed = @MatchesPlayed,
                    MatchesWon = @MatchesWon,
                    MatchesLost = @MatchesLost,
                    MatchesTied = @MatchesTied,
                    Points = @Points,
                    NetRunRate = @NetRunRate
                WHERE Id = @Id",
                standing);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "DELETE FROM TeamStandings WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<IEnumerable<TeamStanding>> GetAllStandingsAsync()
        {
            using var connection = _context.CreateConnection();
            var standings = await connection.QueryAsync<TeamStanding>(
                @"SELECT ts.*, t.Name AS TeamName, tr.Name AS TournamentName 
                FROM TeamStandings ts
                JOIN Teams t ON ts.TeamId = t.Id
                JOIN Tournaments tr ON ts.TournamentId = tr.Id
                ORDER BY tr.Name, ts.Points DESC, ts.NetRunRate DESC");
            return standings;
        }

        public async Task UpdateStandingsAfterMatchAsync(int matchId)
        {
            using var connection = _context.CreateConnection();
            
            // Get match details
            var match = await connection.QueryFirstOrDefaultAsync(
                @"SELECT TeamAId, TeamBId, Result FROM Matches WHERE Id = @MatchId",
                new { MatchId = matchId });
            
            if (match == null) return;
            
            // Get current standings for both teams
            var teamAStanding = await connection.QueryFirstOrDefaultAsync<TeamStanding>(
                @"SELECT * FROM TeamStandings WHERE TeamId = @TeamId",
                new { TeamId = match.TeamAId });
            
            var teamBStanding = await connection.QueryFirstOrDefaultAsync<TeamStanding>(
                @"SELECT * FROM TeamStandings WHERE TeamId = @TeamId",
                new { TeamId = match.TeamBId });
            
            if (teamAStanding != null && teamBStanding != null)
            {
                // Update matches played
                teamAStanding.MatchesPlayed++;
                teamBStanding.MatchesPlayed++;
                
                // Update based on result
                if (match.Result?.Contains("Team A won") == true)
                {
                    teamAStanding.MatchesWon++;
                    teamBStanding.MatchesLost++;
                    teamAStanding.Points += 2;
                }
                else if (match.Result?.Contains("Team B won") == true)
                {
                    teamBStanding.MatchesWon++;
                    teamAStanding.MatchesLost++;
                    teamBStanding.Points += 2;
                }
                else if (match.Result?.Contains("Tie") == true)
                {
                    teamAStanding.MatchesTied++;
                    teamBStanding.MatchesTied++;
                    teamAStanding.Points += 1;
                    teamBStanding.Points += 1;
                }
                
                // Update standings
                await UpdateAsync(teamAStanding);
                await UpdateAsync(teamBStanding);
            }
        }
    }
}
