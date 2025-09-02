using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface IBallByBallRepository
    {
        Task<IEnumerable<BallByBall>> GetBallsByMatchAsync(int matchId);
        Task<IEnumerable<BallByBall>> GetBallsByInningsAsync(int matchId, int innings);
        Task<BallByBall?> GetLastBallAsync(int matchId);
        Task AddAsync(BallByBall ball);
        Task UpdateAsync(BallByBall ball);
        Task DeleteAsync(int id);
        Task<int> GetTotalBallsInInningsAsync(int matchId, int innings);
        Task<float> GetCurrentOversAsync(int matchId, int innings);
        Task<BallByBall?> GetBallByOverAndBallAsync(int matchId, int innings, int over, int ball);
        Task<BallByBall?> GetByIdAsync(int id);
    }
}
