using CricHeroesClone.Models; // Add this line

namespace CricHeroesClone.Repository
{
    public interface IScoreRepository
    {
        // Define the methods that ScoreRepository should implement, for example:
        Task<IEnumerable<Score>> GetScoresByMatchAsync(int matchId);
        Task UpdateScoreAsync(Score score);
    }
}
