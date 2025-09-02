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
                    TeamName = reader.GetString(3),
                    BattingStyle = reader.IsDBNull(4) ? null : reader.GetString(4),
                    BowlingStyle = reader.IsDBNull(5) ? null : reader.GetString(5),
                    TeamId = reader.GetInt32(6)
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

        public async Task UpdateAsync(Player player)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE Players SET Name = @Name, Role = @Role, TeamId = @TeamId, BattingStyle = @BattingStyle, BowlingStyle = @BowlingStyle WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", player.Id);
            cmd.Parameters.AddWithValue("@Name", player.Name);
            cmd.Parameters.AddWithValue("@Role", (object?)player.Role ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TeamId", player.TeamId);
            cmd.Parameters.AddWithValue("@BattingStyle", (object?)player.BattingStyle ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BowlingStyle", (object?)player.BowlingStyle ?? DBNull.Value);
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

        public async Task<int> GetTotalPlayersAsync()
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM Players", con);
            await con.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<IEnumerable<Player>> GetPlayersByTeamAsync(int teamId)
        {
            var list = new List<Player>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT Id, Name, Role, TeamId, BattingStyle, BowlingStyle FROM Players WHERE TeamId = @TeamId", con);
            cmd.Parameters.AddWithValue("@TeamId", teamId);
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Player
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Role = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TeamId = reader.GetInt32(3),
                    BattingStyle = reader.IsDBNull(4) ? null : reader.GetString(4),
                    BowlingStyle = reader.IsDBNull(5) ? null : reader.GetString(5)
                });
            }
            return list;
        }

        public async Task<Player?> GetByIdAsync(int playerId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT Id, Name, Role, TeamId, BattingStyle, BowlingStyle FROM Players WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", playerId);
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Player
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Role = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TeamId = reader.GetInt32(3),
                    BattingStyle = reader.IsDBNull(4) ? null : reader.GetString(4),
                    BowlingStyle = reader.IsDBNull(5) ? null : reader.GetString(5)
                };
            }
            return null;
        }
    }
}
