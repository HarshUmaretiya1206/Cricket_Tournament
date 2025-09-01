using System.Collections.Generic;
using System.Threading.Tasks;
using CricHeroesClone.Models;

namespace CricHeroesClone.Repository
{
    public interface ITournamentRepository
    {
        Task<IEnumerable<Tournament>> GetAllAsync();
        Task AddAsync(Tournament tournament);
        Task DeleteAsync(int id);
        Task<int> GetTotalTournamentsAsync();
        Task<Tournament?> GetByIdAsync(int id);
    }
}
