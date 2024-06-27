using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zufanci.Server.Service;
using Zufanci.Shared;

namespace Zufanci.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly ImageService imageService;

        public ProductsController(IDbContextFactory<ApplicationDbContext> contextFactory, ImageService imageService)
        {
            this.contextFactory = contextFactory;
            this.imageService = imageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var products = await context.Products.Include(u => u.Category).OrderBy(x => x.Name).ToListAsync();
                if (products == null) { return NotFound(); }
                return Ok(products);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var product = await context.Products.Include(v => v.Category).FirstOrDefaultAsync(x => x.Id == id);
                if (product == null) { return NotFound(); }
                return Ok(product);
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Product product)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Add(product);
                await context.SaveChangesAsync();
                return Ok(product.Id);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Product product)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                await imageService.DeleteImage(product);
                string? productImage = product.ImageName == "images/default.png" ? null : product.ImageName;
                Product prod = new Product()
                {
                    Id = product.Id,
                    Name = product.Name,
                    CategoryId = product.CategoryId,
                    ImageName = productImage,
                    LowestPrice = product.LowestPrice,
                    HighestPrice = product.HighestPrice,
                    AveragePrice = product.AveragePrice
                };
                context.Attach(prod).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product == null) { return NotFound(); }
                await imageService.DeleteImage(product);
                context.Remove(product);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
