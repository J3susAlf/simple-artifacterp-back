using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    public class QuotationRepository
    {
        private readonly IMongoCollection<Quotation> _collection;

        public QuotationRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Quotation>("Quotation");
        }

        public Task<List<Quotation>> FindAllAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task<Quotation?> FindByIdAsync(int quotationId)
        {
            return _collection.Find(q => q.QuotationId == quotationId).FirstOrDefaultAsync();
        }

        public Task InsertAsync(Quotation quotation)
        {
            return _collection.InsertOneAsync(quotation);
        }

        public Task UpdateAsync(Quotation quotation)
        {
            return _collection.ReplaceOneAsync(q => q.QuotationId == quotation.QuotationId, quotation);
        }

        public Task DeleteAsync(int quotationId)
        {
            return _collection.DeleteOneAsync(q => q.QuotationId == quotationId);
        }

        public async Task<int> GetNextIdAsync()
        {
            var last = await _collection.Find(_ => true)
                .SortByDescending(q => q.QuotationId)
                .Limit(1)
                .FirstOrDefaultAsync();

            return (last?.QuotationId ?? 0) + 1;
        }
    }
}
