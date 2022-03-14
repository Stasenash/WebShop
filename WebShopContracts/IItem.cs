namespace WebShopContracts
{
    public interface IItemAdded
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }

    public interface IItemUpdated
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }

    public interface IItemDeleted
    {
        public int ItemId { get; set; }
        public int CategoryId { get; set; }
    }
}