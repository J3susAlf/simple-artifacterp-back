using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class AssetsRepository : IAssetsRepository
    {
        private readonly IMongoCollection<Assets> _assets;

        public AssetsRepository(IMongoDatabase database)
        {
            _assets = database.GetCollection<Assets>("Assets");
        }

        public Task<List<Assets>> GetAllAsync()
            => _assets.Find(_ => true).ToListAsync();

        public async Task<Assets?> GetByIdAsync(int id)
        {
            return await _assets.Find(x => x.AssetsId == id).FirstOrDefaultAsync();
        }

        public Task CreateAsync(Assets asset)
            => _assets.InsertOneAsync(asset);

        public Task UpdateAsync(int id, Assets asset)
            => _assets.ReplaceOneAsync(x => x.AssetsId == id, asset);

        public Task DeleteAsync(int id)
            => _assets.DeleteOneAsync(x => x.AssetsId == id);
    }
}