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

        public List<Category> Get()
        {
            return _categories.Find(x => x.ParentId == null).ToList();
        } 

        public Category Get(string id) => _categories.Find(x => x.Id == id).FirstOrDefault();
    }
}
