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
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<ChildCategory> ChildCategories { get; set; } = new List<ChildCategory>();
        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class ChildCategory
    {
        public int Id { get; set; }
        public string Name { set; get; }
    }
}
