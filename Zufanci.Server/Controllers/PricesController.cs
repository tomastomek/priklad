using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zufanci.Shared;

namespace Zufanci.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : Controller
    {
        IDbContextFactory<ApplicationDbContext> contextFactory;

        public PricesController(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<ActionResult<ProductPrice>> Get()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var prices = await context.ProductPrices.Include(x => x.Shop).ToListAsync();
                if (prices.Count > 0)
                {
                    return Ok(prices);
                }
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<ProductPrice>>> Get(int? id = null)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                if (id != null && id > 0)
                {
                    var productPrices = await context.ProductPrices
                        .Include(x => x.Shop)
                        .Include(x => x.Unit)
                        .Where(x => x.ProductId == id.Value)
                        .ToListAsync();

                    if (productPrices != null && productPrices.Count > 0)
                    {
                        return Ok(productPrices);
                    }
                }
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(ProductPrice productPrice)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Add(productPrice);
                await context.SaveChangesAsync();
                return Ok(productPrice.Id);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(ProductPrice productPrice)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var entry = context.Entry(productPrice);
                bool isTracked = entry.State != EntityState.Detached;
                ProductPrice newPrice = new ProductPrice()
                {
                    Id = productPrice.Id,
                    PurchaseDate = productPrice.PurchaseDate,
                    ProductId = productPrice.ProductId,
                    Size = productPrice.Size,
                    Price = productPrice.Price,
                    Discount = productPrice.Discount,
                    ShopId = productPrice.ShopId,
                    UnitId = productPrice.UnitId
                };
                context.Attach(newPrice).State = EntityState.Modified;
                //context.Update(productPrice);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var price = await context.ProductPrices.FirstOrDefaultAsync(x => x.Id == id);
                if (price == null) { return NotFound(); }
                context.Remove(price);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
