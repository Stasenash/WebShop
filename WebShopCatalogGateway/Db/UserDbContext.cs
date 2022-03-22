using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebShopCatalogGateway.Db
{
    public class UserDbContext : DbContext
    {
        private readonly string connectionString;

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public UserDbContext(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("UserDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(x => x.Roles)
                .WithMany(x => x.Users);

            modelBuilder.Entity<Role>()
                .HasData(new[] {
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "User" }
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        /// <summary>
        /// Роли пользователей
        /// </summary>
        public ICollection<Role> Roles { get; set; }
    }

    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Навигационное поле
        /// </summary>
        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
