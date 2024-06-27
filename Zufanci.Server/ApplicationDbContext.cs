using Microsoft.EntityFrameworkCore;
using Zufanci.Shared;

namespace Zufanci.Server
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<MonitoringItem> MonitoringItems { get; set; }
        public DbSet<MonitoringDetail> MonitoringDetails { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
