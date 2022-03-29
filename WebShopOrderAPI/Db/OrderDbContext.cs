using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebShopContracts;

namespace WebShopOrderAPI.Db
{
    public class OrderDbContext : DbContext
    {
        private readonly string connectionString;

        public DbSet<Order> Orders { get; set; }
        public DbSet<Aggregate> Aggregates { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }

        public OrderDbContext(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OrderDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(x => x.Id);
            modelBuilder.Entity<Order>()
                .Property(d => d.Status)
                .HasConversion(new EnumToStringConverter<OrderStatus>());

            modelBuilder.Entity<Order>().Ignore(x => x.Items);

            modelBuilder.Entity<Aggregate>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Event>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Snapshot>()
                .HasKey(x => x.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
