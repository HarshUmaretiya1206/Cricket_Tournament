using System.Data;
using Microsoft.Data.SqlClient;
using CricHeroesClone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CricHeroesClone.Repository
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly string _connectionString;

        public ScoreRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Connection string not found.");
        }

        private async Task<bool> MatchExistsAsync(int matchId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM Matches WHERE Id=@MatchId", con);
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            await con.OpenAsync();
            var count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }

        private async Task<bool> TeamExistsAsync(int teamId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM Teams WHERE Id=@TeamId", con);
            cmd.Parameters.AddWithValue("@TeamId", teamId);
            await con.OpenAsync();
            var count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task<IEnumerable<Score>> GetScoresByMatchAsync(int matchId)
        {
            if (!await MatchExistsAsync(matchId))
                return new List<Score>();

            var list = new List<Score>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spGetScores", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Score
                {
                    Id = reader.GetInt32(0),
                    TeamName = reader.GetString(1),
                    Runs = reader.GetInt32(2),
                    Wickets = reader.GetInt32(3),
                    Overs = (float)reader.GetDouble(4)
                });
            }
            return list;
        }

        public async Task UpdateScoreAsync(Score score)
        {
            if (!await MatchExistsAsync(score.MatchId))
                throw new InvalidOperationException($"MatchId {score.MatchId} does not exist.");
            if (!await TeamExistsAsync(score.TeamId))
                throw new InvalidOperationException($"TeamId {score.TeamId} does not exist.");

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spUpdateScore", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MatchId", score.MatchId);
            cmd.Parameters.AddWithValue("@TeamId", score.TeamId);
            cmd.Parameters.AddWithValue("@Runs", score.Runs);
            cmd.Parameters.AddWithValue("@Wickets", score.Wickets);
            cmd.Parameters.AddWithValue("@Overs", score.Overs);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
