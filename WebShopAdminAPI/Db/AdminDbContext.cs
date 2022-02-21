using Microsoft.EntityFrameworkCore;

namespace WebShopAdminAPI.Db
{
    public class AdminDbContext : DbContext
    {
        private readonly string connectionString;

        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }

        public AdminDbContext(IConfigurationRoot configuration)
        {
            connectionString = configuration.GetConnectionString("AdminDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.SubCategories)
                .HasForeignKey(x => x.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(x => x.Category)
                .WithMany(g => g.Items)
                .HasForeignKey(x => x.CategoryId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
