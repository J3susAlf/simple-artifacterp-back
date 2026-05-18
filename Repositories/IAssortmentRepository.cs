using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public interface IAssortmentRepository
    {
        Task<List<Assortment>> GetAllAsync();
        Task<Assortment?> GetByIdAsync(int id);
        Task<int> GetNextIdAsync();
        Task CreateAsync(Assortment assortment);
        Task UpdateAsync(int id, Assortment assortment);
        Task DeleteAsync(int id);
    }
}