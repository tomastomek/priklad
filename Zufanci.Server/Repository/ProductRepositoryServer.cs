using Microsoft.EntityFrameworkCore;
using System;
using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Server.Service;
using Zufanci.Shared;

namespace Zufanci.Server.Repository
{
    public class ProductRepositoryServer : IProductRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly ImageService imageService;

        public ProductRepositoryServer(IDbContextFactory<ApplicationDbContext> contextFactory, ImageService imageService)
        {
            this.contextFactory = contextFactory;
            this.imageService = imageService;
        }

        public async Task CreateProductAsync(Product product)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Add(product);
                var entries = await context.SaveChangesAsync();
                if (entries == 0)
                {
                    throw new ApplicationException("[CREATE] Nothing was written to the database");
                }
            }            
        }

        public async Task DeleteProductAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product == null) { throw new ApplicationException("[DELETE] Product is null"); }
                await imageService.DeleteImage(product);
                context.Remove(product);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Product> GetProductAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var product = await context.Products.Include(v => v.Category).FirstOrDefaultAsync(x => x.Id == id);
                if (product == null) { throw new ApplicationException("[GET] Product is null"); }
                return product;
            }
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                //Include(u => u.ProductPrices)
                var prod = await context.Products.Include(u => u.Category).OrderBy(x => x.Name).ToListAsync();
                if (prod == null) { throw new ApplicationException("[GET] Products list in null"); }
                return prod;
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                await imageService.DeleteImage(product);
                string? productImage = (product.ImageName == "images/default.png") ? null : product.ImageName;
                product.ImageName = productImage;
                context.Update(product);

                //Product prod = new Product()
                //{
                //    Id = product.Id,
                //    Name = product.Name,
                //    CategoryId = product.CategoryId,
                //    ImageName = productImage,
                //    LowestPrice = product.LowestPrice,
                //    HighestPrice = product.HighestPrice,
                //    AveragePrice = product.AveragePrice
                //};
                //context.Attach(prod).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
