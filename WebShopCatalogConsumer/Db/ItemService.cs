using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopCatalogConsumer.Db
{
    public class ItemService
    {
        private readonly IMongoCollection<Category> _categories;

        public ItemService(ICatalogDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _categories = database.GetCollection<Category>(settings.CatalogCollectionName);
        }

        public Item Create(int categoryId, Item item)
        {
            var category = _categories.Find(x => x.Id == categoryId).FirstOrDefault();
            if (category == null) throw new KeyNotFoundException();

            
            if (!category.Items.Any(x => x.Id == item.Id))
            {
                category.Items.Add(item);
                _categories.ReplaceOne(x => x.Id == category.Id, category);
            }
            return item;
        }

        public void Update(int categoryId, Item item)
        {
            var category = _categories.Find(x => x.Id == categoryId).FirstOrDefault();
            if (category == null) throw new KeyNotFoundException();

            var itemOld = category.Items.FirstOrDefault(x => x.Id == item.Id);
            category.Items.Remove(itemOld);
            category.Items.Add(item);

            _categories.ReplaceOne(x => x.Id == category.Id, category);
        }

        public void Remove(int categoryId, int itemId)
        {
            var category = _categories.Find(x => x.Id == categoryId).FirstOrDefault();
            if (category == null) throw new KeyNotFoundException();

            var itemToDelete = category.Items.FirstOrDefault(x => x.Id == itemId);
            category.Items.Remove(itemToDelete);

            _categories.ReplaceOne(x => x.Id == category.Id, category);
        }
    }
}
