using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface IUserRepository
    {
        Task RegisterAsync(User user);
        Task<User?> LoginAsync(string username, string password);
        Task<IEnumerable<User>> GetAllAsync();
        Task<int> GetTotalUsersAsync();
        Task UpdateUserRoleAsync(int userId, string newRole);
        Task DeleteAsync(int userId);
        Task<User?> GetByIdAsync(int userId);
    }
}
