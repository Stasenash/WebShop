namespace WebShopContracts
{
    public interface ICategoryCreated
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

    public interface ICategoryUpdated
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

    public interface ICategoryDeleted
    {
        public int Id { get; set; }
    }
}