using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zufanci.Shared;

namespace Zufanci.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public UnitsController(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<ActionResult<List<Unit>>> Get()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return Ok(await context.Units.ToListAsync());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Unit>> Get(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var unit = await context.Units.FirstOrDefaultAsync(x => x.Id == id);
                if (unit == null) { return NotFound(); }
                return Ok(unit);
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Unit unit)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Units.Add(unit);
                await context.SaveChangesAsync();
                return Ok(unit.Id);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Unit unit)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Update(unit);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var unit = await context.Units.FirstOrDefaultAsync(x => x.Id == id);
                var productPrice = await context.ProductPrices.Where(x => x.UnitId == id).ToListAsync();
                if (productPrice.Any()) { return Conflict(); }
                if (unit == null) { return NotFound(); }
                context.Remove(unit);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
