using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class AssetsQuotationRepository
    {
        private readonly IMongoCollection<AssetsQuotation> _collection;

        public AssetsQuotationRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<AssetsQuotation>("AssetsQuotation");
        }

        public Task<List<AssetsQuotation>> FindByQuotationVersionIdAsync(int quotationVersionId)
        {
            return _collection.Find(a => a.QuotationVersionId == quotationVersionId).ToListAsync();
        }

        public Task InsertManyAsync(IEnumerable<AssetsQuotation> items)
        {
            return _collection.InsertManyAsync(items);
        }

        public Task DeleteByQuotationVersionIdAsync(int quotationVersionId)
        {
            return _collection.DeleteManyAsync(a => a.QuotationVersionId == quotationVersionId);
        }

        public async Task<int> GetNextIdAsync()
        {
            var last = await _collection.Find(_ => true)
                .SortByDescending(a => a.AssetsQuotationId)
                .Limit(1)
                .FirstOrDefaultAsync();

            return (last?.AssetsQuotationId ?? 0) + 1;
        }
    }
}
