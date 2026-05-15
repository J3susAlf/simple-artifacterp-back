using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class UnitsMeasurementRepository
    {
        private readonly IMongoCollection<UnitsMeasurement> _collection;

        public UnitsMeasurementRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<UnitsMeasurement>("UnitsMeasurement");
        }

        public Task<List<UnitsMeasurement>> FindAllAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task<UnitsMeasurement?> FindByIdAsync(string id)
        {
            return _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public Task InsertAsync(UnitsMeasurement unit)
        {
            return _collection.InsertOneAsync(unit);
        }

        public Task UpdateAsync(UnitsMeasurement unit)
        {
            return _collection.ReplaceOneAsync(u => u.Id == unit.Id, unit);
        }

        public Task DeleteAsync(string id)
        {
            return _collection.DeleteOneAsync(u => u.Id == id);
        }
    }
}
