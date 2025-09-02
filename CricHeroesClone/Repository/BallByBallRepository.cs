using CricHeroesClone.Models;
using CricHeroesClone.Data;
using Dapper;
using System.Data;

namespace CricHeroesClone.Repository
{
    public class BallByBallRepository : IBallByBallRepository
    {
        private readonly DapperContext _context;

        public BallByBallRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BallByBall>> GetBallsByMatchAsync(int matchId)
        {
            using var connection = _context.CreateConnection();
            var balls = await connection.QueryAsync<BallByBall>(
                "spGetBallsByMatch",
                new { MatchId = matchId },
                commandType: CommandType.StoredProcedure);
            return balls;
        }

        public async Task<IEnumerable<BallByBall>> GetBallsByInningsAsync(int matchId, int innings)
        {
            using var connection = _context.CreateConnection();
            var balls = await connection.QueryAsync<BallByBall>(
                "spGetBallsByMatch",
                new { MatchId = matchId },
                commandType: CommandType.StoredProcedure);
            return balls.Where(b => b.Innings == innings);
        }

        public async Task<BallByBall?> GetLastBallAsync(int matchId)
        {
            using var connection = _context.CreateConnection();
            var balls = await connection.QueryAsync<BallByBall>(
                "spGetBallsByMatch",
                new { MatchId = matchId },
                commandType: CommandType.StoredProcedure);
            return balls.OrderByDescending(b => b.Timestamp).FirstOrDefault();
        }

        public async Task AddAsync(BallByBall ball)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "spAddBall",
                new
                {
                    ball.MatchId,
                    ball.Innings,
                    ball.OverNumber,
                    ball.BallNumber,
                    ball.Runs,
                    ball.IsWicket,
                    ball.IsWide,
                    ball.IsNoBall,
                    ball.IsBye,
                    ball.IsLegBye,
                    ball.BatsmanId,
                    ball.BowlerId,
                    ball.WicketTakerId,
                    ball.WicketType,
                    ball.WicketDescription,
                    ball.ExtraRuns,
                    ball.ExtraType,
                    ball.Commentary
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(BallByBall ball)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                @"UPDATE BallByBall SET 
                    Innings = @Innings,
                    OverNumber = @OverNumber,
                    BallNumber = @BallNumber,
                    Runs = @Runs,
                    IsWicket = @IsWicket,
                    IsWide = @IsWide,
                    IsNoBall = @IsNoBall,
                    IsBye = @IsBye,
                    IsLegBye = @IsLegBye,
                    BatsmanId = @BatsmanId,
                    BowlerId = @BowlerId,
                    WicketTakerId = @WicketTakerId,
                    WicketType = @WicketType,
                    WicketDescription = @WicketDescription,
                    ExtraRuns = @ExtraRuns,
                    ExtraType = @ExtraType,
                    Commentary = @Commentary,
                    Timestamp = GETDATE()
                WHERE Id = @Id",
                ball);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "DELETE FROM BallByBall WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<int> GetTotalBallsInInningsAsync(int matchId, int innings)
        {
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM BallByBall WHERE MatchId = @MatchId AND Innings = @Innings",
                new { MatchId = matchId, Innings = innings });
            return count;
        }

        public async Task<float> GetCurrentOversAsync(int matchId, int innings)
        {
            using var connection = _context.CreateConnection();
            var totalBalls = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM BallByBall WHERE MatchId = @MatchId AND Innings = @Innings",
                new { MatchId = matchId, Innings = innings });
            return (float)Math.Round(totalBalls / 6.0f + (totalBalls % 6) / 10.0f, 1);
        }

        public async Task<BallByBall?> GetBallByOverAndBallAsync(int matchId, int innings, int over, int ball)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<BallByBall>(
                @"SELECT * FROM BallByBall 
                WHERE MatchId = @MatchId AND Innings = @Innings 
                AND OverNumber = @OverNumber AND BallNumber = @BallNumber",
                new { MatchId = matchId, Innings = innings, OverNumber = over, BallNumber = ball });
            return result;
        }

        public async Task<BallByBall?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<BallByBall>(
                "SELECT * FROM BallByBall WHERE Id = @Id",
                new { Id = id });
            return result;
        }
    }
}
