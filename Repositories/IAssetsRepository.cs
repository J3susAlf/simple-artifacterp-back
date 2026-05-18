using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public interface IAssetsRepository
    {
        Task<List<Assets>> GetAllAsync();
        Task<Assets?> GetByIdAsync(int id);
        Task CreateAsync(Assets asset);
        Task UpdateAsync(int id, Assets asset);
        Task DeleteAsync(int id);
    }
}