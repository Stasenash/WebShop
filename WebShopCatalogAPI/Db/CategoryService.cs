﻿using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopCatalogAPI.Db
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

        public Category Get()
        {
            var categories = _categories.Find(x => x.ParentId == null).ToList();

            return new Category
            {
                Id = string.Empty,
                RelationalId = -1,
                Name = "Catalog",
                ChildCategories = categories.Select(x => new ChildCategory { Id = x.RelationalId, Name = x.Name }).ToList()
            };
        }

        public Category Get(int id) => _categories.Find(x => x.RelationalId == id).FirstOrDefault();
    }
}
