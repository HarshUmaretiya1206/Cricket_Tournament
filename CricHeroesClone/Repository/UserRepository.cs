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
                new { UserName = user.Username, Email = user.Email, PasswordHash = user.Password },
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
    }
}
