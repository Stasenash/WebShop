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

        public Category Get(int id) => _categories.Find(x => x.Id == id).FirstOrDefault();

        public Category Create(Category category)
        {
            _categories.InsertOne(category);
            AddChildCategory(category.ParentId, category.Id, category.Name);

            return category;
        }

        public void Update(Category category)
        {
            var oldCategory = _categories.Find(x => x.Id == category.Id).FirstOrDefault();
            oldCategory.Name = category.Name;
            oldCategory.ParentId = category.ParentId;

            if (oldCategory.ParentId != category.ParentId)
            {
                RemoveChildCategory(oldCategory.ParentId, category.Id);
                AddChildCategory(category.ParentId, category.Id, category.Name);
            }

            _categories.ReplaceOne(x => x.Id == oldCategory.Id, oldCategory);
        }

        public void Remove(int categoryId)
        {
            var category = _categories.Find(x => x.Id == categoryId).FirstOrDefault();
            if (category == null) return;

            _categories.DeleteOne(x => x.Id == categoryId);
            RemoveChildCategory(category.ParentId, categoryId);
        }

        private void AddChildCategory(int? parentId, int childId, string childName)
        {
            if (!parentId.HasValue) return;

            var parentCategory = _categories.Find(x => x.Id == parentId).FirstOrDefault();
            if (parentCategory == null) return;

            parentCategory.ChildCategories.Add(new ChildCategory { Id = childId, Name = childName });
            _categories.ReplaceOne(x => x.Id == parentCategory.Id, parentCategory);
        }

        private void RemoveChildCategory(int? parentId, int childId)
        {
            if (!parentId.HasValue) return;

            var parentCategory = _categories.Find(x => x.Id == parentId).FirstOrDefault();
            if (parentCategory == null) return;

            parentCategory.ChildCategories.RemoveAll(x => x.Id == childId);
            _categories.ReplaceOne(x => x.Id == parentCategory.Id, parentCategory);
        }
    }
}
