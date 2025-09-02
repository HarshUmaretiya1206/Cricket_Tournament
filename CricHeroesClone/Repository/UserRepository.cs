using CricHeroesClone.Data;
using CricHeroesClone.Models;
using Dapper;

namespace CricHeroesClone.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        public UserRepository(DapperContext context) => _context = context;

        public async Task RegisterAsync(User user)
        {
            using var con = _context.CreateConnection();
            await con.ExecuteAsync("spRegisterUser",
                new { UserName = user.Username, Email = user.Email, PasswordHash = user.PasswordHash, Role = user.Role },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            using var con = _context.CreateConnection();
            var user = await con.QueryFirstOrDefaultAsync<User>("spLoginUser",
                new { UserName = username, PasswordHash = password },
                commandType: System.Data.CommandType.StoredProcedure);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var con = _context.CreateConnection();
            return await con.QueryAsync<User>("SELECT * FROM Users");
        }

        public async Task<int> GetTotalUsersAsync()
        {
            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
        }

        public async Task EnsureDefaultAdminAsync()
        {
            using var con = _context.CreateConnection();
            var exists = await con.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Users WHERE UserName = @u", new { u = "admin" });
            if (exists == 0)
            {
                await con.ExecuteAsync(
                    "spRegisterUser",
                    new { UserName = "admin", Email = "admin@cricheroes.com", PasswordHash = "admin123", Role = "Admin" },
                    commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task UpdateUserRoleAsync(int userId, string newRole)
        {
            using var con = _context.CreateConnection();
            await con.ExecuteAsync("UPDATE Users SET Role = @Role WHERE Id = @Id",
                new { Role = newRole, Id = userId });
        }

        public async Task DeleteAsync(int userId)
        {
            using var con = _context.CreateConnection();
            await con.ExecuteAsync("DELETE FROM Users WHERE Id = @Id",
                new { Id = userId });
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            using var con = _context.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id",
                new { Id = userId });
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var con = _context.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE UserName = @UserName",
                new { UserName = username });
        }
    }
}
