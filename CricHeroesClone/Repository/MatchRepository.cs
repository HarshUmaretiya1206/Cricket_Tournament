using System.Data;
using Microsoft.Data.SqlClient;
using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public class MatchRepository : IMatchRepository
    {
        private readonly string _connectionString;

        public MatchRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new ArgumentNullException("DefaultConnection is missing in appsettings.json");
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            var list = new List<Match>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spGetMatches", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Match
                {
                    Id = reader.GetInt32(0),
                    TournamentName = reader.GetString(1),
                    TeamA = reader.GetString(2),
                    TeamB = reader.GetString(3),
                    MatchDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    Venue = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Result = reader.IsDBNull(6) ? "" : reader.GetString(6)
                });
            }
            return list;
        }

        public async Task<Match?> GetByIdAsync(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spGetMatches", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32(0) == id)
                {
                    return new Match
                    {
                        Id = reader.GetInt32(0),
                        TournamentName = reader.GetString(1),
                        TeamA = reader.GetString(2),
                        TeamB = reader.GetString(3),
                        MatchDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                        Venue = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        Result = reader.IsDBNull(6) ? "" : reader.GetString(6)
                    };
                }
            }
            return null;
        }

        public async Task AddAsync(Match match)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spAddMatch", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@TournamentId", match.TournamentId);
            cmd.Parameters.AddWithValue("@TeamAId", match.TeamAId);
            cmd.Parameters.AddWithValue("@TeamBId", match.TeamBId);
            cmd.Parameters.AddWithValue("@MatchDate", (object?)match.MatchDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Venue", (object?)match.Venue ?? DBNull.Value);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Match match)
        {
            // You can create an spUpdateMatch later if needed
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spDeleteMatch", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // ✅ Get both teams' scores for a match
        public async Task<MatchScoreDto?> GetScoreAsync(int matchId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spGetScores", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MatchId", matchId);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            string? teamAName = null;
            string? teamBName = null;
            string? scoreA = null;
            string? scoreB = null;

            int count = 0;
            while (await reader.ReadAsync())
            {
                string teamName = reader.GetString(1); // TeamName
                int runs = reader.GetInt32(2);
                int wickets = reader.GetInt32(3);
                double overs = reader.GetDouble(4);

                string score = $"{runs}/{wickets} in {overs} overs";

                if (count == 0)
                {
                    teamAName = teamName;
                    scoreA = score;
                }
                else if (count == 1)
                {
                    teamBName = teamName;
                    scoreB = score;
                }

                count++;
            }

            return new MatchScoreDto
            {
                MatchId = matchId,
                TeamAName = teamAName ?? "Team A",
                TeamBName = teamBName ?? "Team B",
                ScoreA = scoreA ?? "0/0 in 0 overs",
                ScoreB = scoreB ?? "0/0 in 0 overs"
            };
        }

    }
}

