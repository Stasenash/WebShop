using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopContracts;

namespace WebShopOrderAPI.Db
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public string Items { get; set; }
    }

    public class Aggregate
    {
        [Key]
        public int Id { get; set; }
        public Guid AggregateId { get; set; }
        public int OrderId { get; set; }
        public int OwnerId { get; set; }
        public int Version { get; set; }

    }

    public class Event
    {
        [Key]
        public int Id { get; set; }
        public Guid AggregateId { get; set; }
        public byte[] EventData { get; set; }
        public int Version { get; set; }
    }

    public class Snapshot
    {
        [Key]
        public int Id { get; set; }
        public Guid AggregateId { get; set; }
        public byte[] SerializedData { get; set; }
        public int Version { get; set; }
    }
}
