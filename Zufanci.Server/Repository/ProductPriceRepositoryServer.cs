using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Shared;

namespace Zufanci.Server.Repository
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductPriceRepositoryServer : IProductPriceRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public ProductPriceRepositoryServer(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task CreatePricesAsync(ProductPrice productPrice)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Add(productPrice);
                var entries = await context.SaveChangesAsync();
                if (entries == 0)
                {
                    throw new ApplicationException("Nothing was written to the database");
                }
            }
        }

        public async Task DeletePricesAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var price = await context.ProductPrices.FirstOrDefaultAsync(x => x.Id == id);
                if (price == null) { throw new ApplicationException("[DELETE] Price is null"); }
                context.Remove(price);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ProductPrice>> GetAllPricesAsync()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var prices = await context.ProductPrices.Include(x => x.Shop).ToListAsync();
                if (prices.Count > 0)
                {
                    return prices;
                }
                else
                {
                    return new List<ProductPrice> { };
                }
            }
        }

        public async Task<List<ProductPrice>> GetPricesAsync(int? id = null)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                if (id != null & id > 0)
                {
                    return await context.ProductPrices
                        .Include(x => x.Shop)
                        .Include(x => x.Unit)
                        .Where(x => x.ProductId == id.Value)
                        .ToListAsync();
                }
                else
                {
                    throw new ApplicationException($"[GET] Cannot find price with id {id}.");
                }
            }
        }

        public async Task UpdatePricesAsync(ProductPrice productPrice)
        {
            //ProductPrice newPrice = new ProductPrice()
            //{
            //    Id = productPrice.Id,
            //    PurchaseDate = productPrice.PurchaseDate,
            //    ProductId = productPrice.ProductId,
            //    Size = productPrice.Size,
            //    Price = productPrice.Price,
            //    Discount = productPrice.Discount,
            //    ShopId = productPrice.ShopId,
            //    UnitId = productPrice.UnitId
            //};
            //context.Attach(newPrice).State = EntityState.Modified;
            using (var context = contextFactory.CreateDbContext())
            {
                context.Update(productPrice);
                await context.SaveChangesAsync();
            }
        }
    }
}
