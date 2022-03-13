using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopCatalogConsumer.Db
{
    public class CategoryService
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoryService(ICatalogDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _categories = database.GetCollection<Category>(settings.CatalogCollectionName);
        }

        public List<Category> Get() => _categories.Find(Category => true).ToList();

        public Category Get(string id) => _categories.Find(x => x.Id == id).FirstOrDefault();

        public Category Create(Category category)
        {
            _categories.InsertOne(category);
            return category;
        }

        public void Update(string id, Category categoryIn) => _categories.ReplaceOne(x => x.Id == id, categoryIn);

        public void Remove(Category categoryIn) => _categories.DeleteOne(x => x.Id == categoryIn.Id);

        public void Remove(string id) => _categories.DeleteOne(x => x.Id == id);
    }
}
