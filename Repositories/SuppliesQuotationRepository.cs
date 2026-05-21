using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class SuppliesQuotationRepository
    {
        private readonly IMongoCollection<SuppliesQuotation> _collection;

        public SuppliesQuotationRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SuppliesQuotation>("SuppliesQuotation");
        }

        public Task<List<SuppliesQuotation>> FindByQuotationVersionIdAsync(int quotationVersionId)
        {
            return _collection.Find(s => s.QuotationVersionId == quotationVersionId).ToListAsync();
        }

        public Task InsertManyAsync(IEnumerable<SuppliesQuotation> items)
        {
            return _collection.InsertManyAsync(items);
        }

        public Task DeleteByQuotationVersionIdAsync(int quotationVersionId)
        {
            return _collection.DeleteManyAsync(s => s.QuotationVersionId == quotationVersionId);
        }

        public async Task<int> GetNextIdAsync()
        {
            var last = await _collection.Find(_ => true)
                .SortByDescending(s => s.SuppliesQuotationId)
                .Limit(1)
                .FirstOrDefaultAsync();

            return (last?.SuppliesQuotationId ?? 0) + 1;
        }
    }
}
