using Microsoft.EntityFrameworkCore;

namespace WebShopAdminAPI.Db
{
    public class AdminDbContext : DbContext
    {
        private readonly string connectionString;

        public DbSet<Category> Categories { get; set; }

        public AdminDbContext(IConfigurationRoot configuration)
        {
            connectionString = configuration.GetConnectionString("AdminDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name);
                entity.HasOne(x => x.Parent)
                    .WithMany(x => x.SubCategories)
                    .HasForeignKey(x => x.ParentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
