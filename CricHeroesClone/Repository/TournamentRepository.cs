using System.Collections.Generic;
using System.Threading.Tasks;
using CricHeroesClone.Models;
using CricHeroesClone.Data;
using Dapper;
using System.Data;

namespace CricHeroesClone.Repository
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly DapperContext _context;

        public TournamentRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tournament>> GetAllAsync()
        {
            var query = "SELECT * FROM Tournaments";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Tournament>(query);
            }
        }

        public async Task AddAsync(Tournament tournament)
        {
            var query = "INSERT INTO Tournaments (Name, StartDate, EndDate) VALUES (@Name, @StartDate, @EndDate)";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, tournament);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM Tournaments WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
            }
        }

        public async Task<int> GetTotalTournamentsAsync()
        {
            var query = "SELECT COUNT(*) FROM Tournaments";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<Tournament?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Tournaments WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Tournament>(query, new { Id = id });
            }
        }
    }
}
