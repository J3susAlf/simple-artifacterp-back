using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class SuppliesRepository
    {
        private readonly IMongoCollection<Supplies> _collection;

        public SuppliesRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Supplies>("Supplies");
        }

        public Task<List<Supplies>> FindAllAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task<Supplies?> FindByIdAsync(string id)
        {
            return _collection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public Task InsertAsync(Supplies supply)
        {
            return _collection.InsertOneAsync(supply);
        }

        public Task UpdateAsync(Supplies supply)
        {
            return _collection.ReplaceOneAsync(s => s.Id == supply.Id, supply);
        }

        public Task DeleteAsync(string id)
        {
            return _collection.DeleteOneAsync(s => s.Id == id);
        }
    }
}
