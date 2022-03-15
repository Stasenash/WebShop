using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopCatalogAPI.Db
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int RelationalId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<ChildCategory> ChildCategories { get; } = new List<ChildCategory>();
        public List<Item> Items { get; } = new List<Item>();
    }

    public class ChildCategory
    {
        public int Id { get; set; }
        public string Name { set; get; }
    }
}
