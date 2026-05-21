using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class QuotationVersionRepository
    {
        private readonly IMongoCollection<QuotationVersion> _collection;

        public QuotationVersionRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<QuotationVersion>("QuotationVersion");
        }

        public Task<List<QuotationVersion>> FindByQuotationIdAsync(int quotationId)
        {
            return _collection.Find(v => v.QuotationId == quotationId).ToListAsync();
        }

        public Task<QuotationVersion?> FindLatestByQuotationIdAsync(int quotationId)
        {
            return _collection.Find(v => v.QuotationId == quotationId)
                .SortByDescending(v => v.VersionNumber)
                .Limit(1)
                .FirstOrDefaultAsync();
        }

        public Task InsertAsync(QuotationVersion version)
        {
            return _collection.InsertOneAsync(version);
        }

        public Task UpdateAsync(QuotationVersion version)
        {
            return _collection.ReplaceOneAsync(v => v.QuotationVersionId == version.QuotationVersionId, version);
        }

        public async Task<int> GetNextIdAsync()
        {
            var last = await _collection.Find(_ => true)
                .SortByDescending(v => v.QuotationVersionId)
                .Limit(1)
                .FirstOrDefaultAsync();

            return (last?.QuotationVersionId ?? 0) + 1;
        }
    }
}
