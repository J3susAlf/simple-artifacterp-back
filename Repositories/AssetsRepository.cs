using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Repositories
{
    {

        public AssetsRepository(IMongoDatabase database)
        {
        }


        {
            return _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }



    }
}