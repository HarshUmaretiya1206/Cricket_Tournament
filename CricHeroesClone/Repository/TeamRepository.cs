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
                    TournamentName = reader.GetString(2)
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
    }
}