using System.Data;
using Microsoft.Data.SqlClient;
using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly string _connectionString;

        public PlayerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            var list = new List<Player>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spGetPlayers", con) { CommandType = CommandType.StoredProcedure };
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Player
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Role = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TeamName = reader.GetString(3)
                });
            }
            return list;
        }

        public async Task AddAsync(Player player)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spAddPlayer", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Name", player.Name);
            cmd.Parameters.AddWithValue("@Role", (object?)player.Role ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TeamId", player.TeamId);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int playerId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spDeletePlayer", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", playerId);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // ✅ Add this method to fetch teams for dropdown
        public async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            var teams = new List<Team>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spGetTeams", con) { CommandType = CommandType.StoredProcedure };
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                teams.Add(new Team
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            return teams;
        }
    }
}
