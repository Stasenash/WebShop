using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopBasketConsumer.Db
{
    public class BasketService
    {
        private readonly IMongoCollection<Basket> _baskets;

        public BasketService(IBasketDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _baskets = database.GetCollection<Basket>(settings.BasketCollectionName);
        }

        public async Task UpdateItems(Item item)
        {
            var baskets = _baskets.Find(x => x.Items.Any(i => i.Id == item.Id)).ToList();
            foreach (var basket in baskets)
            {
                var basketItem = basket.Items.First(i => i.Id == item.Id);
                basketItem.Name = item.Name;
                basketItem.Price = item.Price;
                _baskets.ReplaceOne(x => x.Id == basket.Id, basket);
            }
        }

        public async Task DeleteItems(int itemId)
        {
            var baskets = _baskets.Find(x => x.Items.Any(i => i.Id == itemId)).ToList();
            foreach (var basket in baskets)
            {
                basket.Items.RemoveAll(i => i.Id == itemId);
                _baskets.ReplaceOne(x => x.Id == basket.Id, basket);
            }
        }
    }
}
