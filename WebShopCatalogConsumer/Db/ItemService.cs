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
            var itemOldCategory = _categories.Find(x => x.Items.Any(y => y.Id == item.Id)).FirstOrDefault();
            var itemNewCategory = _categories.Find(x => x.Id == categoryId).FirstOrDefault();
            if (itemNewCategory == null) throw new KeyNotFoundException();
                        
            if (itemOldCategory != null && itemOldCategory.Id != categoryId)
            {
                // если категория у продукта поменялась
                var itemOld = itemOldCategory.Items.FirstOrDefault(x => x.Id == item.Id);

                itemOldCategory.Items.Remove(itemOld);
                itemNewCategory.Items.Add(item);

                _categories.ReplaceOne(x => x.Id == itemOldCategory.Id, itemOldCategory);
                _categories.ReplaceOne(x => x.Id == itemNewCategory.Id, itemNewCategory);
            }
            else
            {
                // если категория не менялась
                var itemOld = itemNewCategory.Items.FirstOrDefault(x => x.Id == item.Id);

                itemNewCategory.Items.Remove(itemOld);
                itemNewCategory.Items.Add(item);

                _categories.ReplaceOne(x => x.Id == itemNewCategory.Id, itemNewCategory);
            }
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
