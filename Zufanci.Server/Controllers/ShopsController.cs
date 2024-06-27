using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zufanci.Server.Service;
using Zufanci.Shared;

namespace Zufanci.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopsController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly ImageService imageService;

        public ShopsController(IDbContextFactory<ApplicationDbContext> contextFactory, ImageService imageService)
        {
            this.contextFactory = contextFactory;
            this.imageService = imageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Shop>>> Get()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return Ok(await context.Shops.OrderBy(x => x.Name).ToListAsync());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shop>> Get(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var shop = await context.Shops.FirstOrDefaultAsync(x => x.Id == id);
                if (shop == null) { return NotFound(); }
                return Ok(shop);
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Shop shop)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Shops.Add(shop);
                await context.SaveChangesAsync();
                return Ok(shop.Id);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Shop shop)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                await imageService.DeleteImage(shop);
                //context.Update(shop);
                context.Attach(shop).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var shop = await context.Shops.FirstOrDefaultAsync(x => x.Id == id);
                var productPrice = await context.ProductPrices.Where(x => x.ShopId == id).ToListAsync();
                if (productPrice.Any()) { return Conflict(); }
                if (shop == null) { return NotFound(); }
                await imageService.DeleteImage(shop);
                context.Remove(shop);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
