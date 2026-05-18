using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class InventorySuppliesRepository
    {
        private readonly IMongoCollection<InventorySupplies> _collection;

        public InventorySuppliesRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<InventorySupplies>("InventorySupplies");
        }

        public Task<InventorySupplies?> FindBySuppliesIdAsync(string suppliesId)
        {
            return _collection.Find(i => i.SuppliesId == suppliesId).FirstOrDefaultAsync();
        }

        public Task<List<InventorySupplies>> FindAllAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task InsertAsync(InventorySupplies inventory)
        {
            return _collection.InsertOneAsync(inventory);
        }

        public Task UpdateAsync(InventorySupplies inventory)
        {
            return _collection.ReplaceOneAsync(i => i.InventorySuppliesId == inventory.InventorySuppliesId, inventory);
        }

        public async Task<int> GetNextIdAsync()
        {
            var last = await _collection.Find(_ => true)
                .SortByDescending(x => x.InventorySuppliesId)
                .Limit(1)
                .FirstOrDefaultAsync();

            return (last?.InventorySuppliesId ?? 0) + 1;
        }
    }
}
