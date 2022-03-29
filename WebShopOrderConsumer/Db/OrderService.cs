using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopContracts;

namespace WebShopOrderConsumer.Db
{
    public class OrderService
    {
        private readonly OrderDbContext _dbContext;
        private readonly IConfigurationRoot _config;

        public OrderService(OrderDbContext dbContext, IConfiguration configRoot)
        {
            _dbContext = dbContext;
            _config = (IConfigurationRoot)configRoot;
        }

        public async Task CreateAggregate(int userId, int orderId)
        {
            // Aggregate
            var aggregateId = Guid.NewGuid();
            var version = 1;
            var aggregate = new Aggregate
            {
                AggregateId = aggregateId,
                OrderId = orderId,
                OwnerId = userId,
                Version = version
            };

            // Event
            var eventData = new
            {
                Status = OrderStatus.Created,
                UserId = userId
            };
            var eventDataSerialized = JsonConvert.SerializeObject(eventData);
            var blobEventData = Encoding.UTF8.GetBytes(eventDataSerialized);
            Event @event = new Event
            {
                AggregateId = Guid.NewGuid(),
                Version = version,
                EventData = blobEventData
            };

            // Snapshot
            var order = _dbContext.Orders.Where(x => x.Id == aggregate.OrderId).First();
            var serialized = JsonConvert.SerializeObject(order);
            var blob = Encoding.UTF8.GetBytes(serialized);
            var snapshot = new Snapshot
            {
                AggregateId = aggregateId,
                Version = version,
                SerializedData = blob
            };


            _dbContext.Aggregates.Add(aggregate);
            _dbContext.Events.Add(@event);
            _dbContext.Snapshots.Add(snapshot);

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAggregate(int orderId, int userId, OrderStatus newStatus)
        {
            // Aggregate
            var orderAggregates = _dbContext.Aggregates.Where(x => x.OrderId == orderId);
            var maxVersion = orderAggregates.Max(x => x.Version);
            var lastAggregate = orderAggregates.Where(x => x.Version == maxVersion).First();

            var aggregateId = lastAggregate.AggregateId;
            var newVersion = lastAggregate.Version + 1;
            lastAggregate.Version = newVersion;

            // Event
            var eventData = new
            {
                Status = newStatus,
                UserId = userId
            };
            var eventDataSerialized = JsonConvert.SerializeObject(eventData);
            var blobEventData = Encoding.UTF8.GetBytes(eventDataSerialized);
            Event @event = new Event
            {
                AggregateId = Guid.NewGuid(),
                Version = newVersion,
                EventData = blobEventData
            };

            // Snapshot
            var order = _dbContext.Orders.Where(x => x.Id == lastAggregate.OrderId).First();
            var serialized = JsonConvert.SerializeObject(order);
            var blob = Encoding.UTF8.GetBytes(serialized);
            var snapshot = new Snapshot
            {
                AggregateId = aggregateId,
                Version = newVersion,
                SerializedData = blob
            };

            _dbContext.Events.Add(@event);
            _dbContext.Snapshots.Add(snapshot);

            await _dbContext.SaveChangesAsync();
        }
    }
}
