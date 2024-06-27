using Microsoft.EntityFrameworkCore;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Server.Service;
using Zufanci.Shared;

namespace Zufanci.Server.Repository
{
    public class ShopRepositoryServer : IShopRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly ImageService imageService;

        public ShopRepositoryServer(IDbContextFactory<ApplicationDbContext> contextFactory, ImageService imageService)
        {
            this.contextFactory = contextFactory;
            this.imageService = imageService;
        }

        public async Task CreateShopAsync(Shop shop)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Shops.Add(shop);
                int entries = await context.SaveChangesAsync();

                if (entries == 0)
                {
                    throw new ApplicationException("Nothing was written to the database");
                }
            }
        }

        public async Task<Shop> GetShopAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var shop = await context.Shops.FirstOrDefaultAsync(x => x.Id == id);
                if (shop == null) { throw new ApplicationException("Shop is null"); }
                return shop;
            }
        }

        public async Task<List<Shop>> GetShopsAsync()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.Shops.OrderBy(x => x.Name).ToListAsync();
            }
        }

        public async Task UpdateShopAsync(Shop shop)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                await imageService.DeleteImage(shop);
                context.Update(shop);
                var entries = await context.SaveChangesAsync();
                if (entries == 0)
                {
                    throw new ApplicationException("Cannot update shop in database");
                }
            }
        }

        public async Task<bool> DeleteShopAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var shop = await context.Shops.FirstOrDefaultAsync(x => x.Id == id);
                var productPrice = await context.ProductPrices.Where(x => x.ShopId == id).ToListAsync();
                if (productPrice.Any()) { return false; }
                if (shop == null) { return false; }
                await imageService.DeleteImage(shop);
                context.Remove(shop);
                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}
