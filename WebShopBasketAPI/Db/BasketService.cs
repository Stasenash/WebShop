using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopBasketAPI.Models;

namespace WebShopBasketAPI.Db
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

        public BasketDto Get(int userId)
        {
            var basket = _baskets.Find(x => x.UserId == userId).FirstOrDefault();
            if (basket == null) return new BasketDto();
            return new BasketDto(basket);
        }

        public void Clear(int userId)
        {
            var basket = _baskets.Find(x => x.UserId == userId).FirstOrDefault();
            _baskets.DeleteOne(x => x.UserId == userId);
        }

        public void AddItem(int userId, Item item)
        {
            item.Count = 1;
            var basket = _baskets.Find(x => x.UserId == userId).FirstOrDefault();
            if (basket == null)
            {
                basket = new Basket { UserId = userId };
                basket.Items.Add(item);
                _baskets.InsertOne(basket);
            }
            else if (!basket.Items.Any(x => x.Id == item.Id))
            {
                basket.Items.Add(item);
                _baskets.ReplaceOne(x => x.UserId == userId, basket);
            }
            else
            {
                var basketItem = basket.Items.FirstOrDefault(x => x.Id == item.Id);
                basketItem.Count++;
                _baskets.ReplaceOne(x => x.UserId == userId, basket);
            }
        }

        public void DeleteItem(int userId, int itemId)
        {
            var basket = _baskets.Find(x => x.UserId == userId).FirstOrDefault();
            if (basket != null && basket.Items.Any(x => x.Id == itemId))
            {
                basket.Items.RemoveAll(x => x.Id == itemId);
                _baskets.ReplaceOne(x => x.UserId == userId, basket);
            }
        }
    }
}
