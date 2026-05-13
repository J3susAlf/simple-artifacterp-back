using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class AssortmentRepository : IAssortmentRepository
    {
        private readonly IMongoCollection<Assortment> _assortments;

        public AssortmentRepository(IMongoDatabase database)
        {
            _assortments = database.GetCollection<Assortment>("Assortment");
        }

        public Task<List<Assortment>> GetAllAsync()
            => _assortments.Find(_ => true).ToListAsync();

        public async Task<Assortment?> GetByIdAsync(int id)
        {
            return await _assortments.Find(x => x.AssortmentId == id).FirstOrDefaultAsync();
        }

        public Task CreateAsync(Assortment assortment)
            => _assortments.InsertOneAsync(assortment);

        public Task UpdateAsync(int id, Assortment assortment)
            => _assortments.ReplaceOneAsync(x => x.AssortmentId == id, assortment);

        public Task DeleteAsync(int id)
            => _assortments.DeleteOneAsync(x => x.AssortmentId == id);
    }
}