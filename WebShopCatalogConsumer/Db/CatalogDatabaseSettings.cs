using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopCatalogConsumer.Db
{
    public class CatalogDatabaseSettings : ICatalogDatabaseSettings
    {
        public string CatalogCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface ICatalogDatabaseSettings
    {
        string CatalogCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
