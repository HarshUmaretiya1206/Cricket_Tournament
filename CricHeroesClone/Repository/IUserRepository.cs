using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface IUserRepository
    {
        Task RegisterAsync(User user);
        Task<User?> LoginAsync(string username, string password);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
