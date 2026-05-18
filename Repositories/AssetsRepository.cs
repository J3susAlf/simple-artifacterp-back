using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class AssetsRepository
    {
        private readonly IMongoCollection<Assets> _collection;

        public AssetsRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Assets>("Assets");
        }

        public Task<List<Assets>> FindAllAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task<Assets?> FindByIdAsync(string id)
        {
            return _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public Task InsertAsync(Assets asset)
        {
            return _collection.InsertOneAsync(asset);
        }

        public Task UpdateAsync(Assets asset)
        {
            return _collection.ReplaceOneAsync(a => a.Id == asset.Id, asset);
        }

        public Task DeleteAsync(string id)
        {
            return _collection.DeleteOneAsync(a => a.Id == id);
        }
    }
}