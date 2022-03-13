using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopCatalogConsumer.Db
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int RelationalId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<Category> SubCategories { get; } = new List<Category>();
        public List<Item> Items { get; } = new List<Item>();
    }
}
