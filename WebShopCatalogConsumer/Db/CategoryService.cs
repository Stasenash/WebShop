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

        public void Update(Category category)
        {
            var oldCategory = _categories.Find(x => x.RelationalId == category.RelationalId).FirstOrDefault();
            oldCategory.Name = category.Name;
            oldCategory.ParentId = category.ParentId;

            _categories.ReplaceOne(x => x.Id == oldCategory.Id, oldCategory);
        }

        public void Remove(int categoryId)
        {
            _categories.DeleteOne(x => x.RelationalId == categoryId);
        }
    }
}
