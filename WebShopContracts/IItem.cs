namespace WebShopContracts
{
    public interface IItemAdded
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }

    public interface IItemUpdated
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }

    public interface IItemDeleted
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
    }
}