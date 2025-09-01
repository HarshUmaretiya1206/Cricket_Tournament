using System.Data;
using Microsoft.Data.SqlClient;
using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    // Implement ITeamRepository interface
    public class TeamRepository : ITeamRepository
    {
        private readonly string _connectionString;

        public TeamRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            var list = new List<Team>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spGetTeams", con) { CommandType = CommandType.StoredProcedure };
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Team
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    TournamentName = reader.GetString(2),
                    CaptainId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    TeamName = reader.GetString(1), // Use Name as TeamName
                    CaptainName = reader.IsDBNull(4) ? "No Captain" : reader.GetString(4)
                });
            }
            return list;
        }

        public async Task AddAsync(Team team)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spAddTeam", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Name", team.Name);
            cmd.Parameters.AddWithValue("@TournamentId", team.TournamentId);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // Match interface signature: DeleteAsync(int teamId)
        public async Task DeleteAsync(int teamId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spDeleteTeam", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", teamId);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> GetTotalTeamsAsync()
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM Teams", con);
            await con.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<Team?> GetTeamByCaptainAsync(int captainId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Teams WHERE CaptainID = @CaptainID", con);
            cmd.Parameters.AddWithValue("@CaptainID", captainId);
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Team
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    TournamentId = reader.GetInt32("TournamentId"),
                    CaptainId = reader.GetInt32("CaptainID")
                };
            }
            return null;
        }

        public async Task<Team?> GetByIdAsync(int teamId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Teams WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", teamId);
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Team
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    TournamentId = reader.GetInt32("TournamentId"),
                    CaptainId = reader.GetInt32("CaptainID")
                };
            }
            return null;
        }

        public async Task UpdateCaptainAsync(int teamId, int? captainId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE Teams SET CaptainId = @CaptainId WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@CaptainId", captainId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", teamId);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}