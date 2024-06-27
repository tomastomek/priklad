using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zufanci.Server.Service;
using Zufanci.Shared;

namespace Zufanci.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly ImageService imageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesController"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="imageService">The image service.</param>
        public CategoriesController(IDbContextFactory<ApplicationDbContext> contextFactory, ImageService imageService)
        {
            this.contextFactory = contextFactory;
            this.imageService = imageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> Get()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return Ok(await context.Categories.OrderBy(x => x.Name).ToListAsync());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null) { return NotFound(); }
                return Ok(category);
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Category category)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();
                return Ok(category.Id);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Category category)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                await imageService.DeleteImage(category);
                context.Update(category);
                //context.Attach(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                var productWithCategory = await context.Products.Where(x => x.CategoryId == id).ToListAsync();
                if (productWithCategory.Any()) { return Conflict(); }
                if (category == null) { return NotFound(); }
                await imageService.DeleteImage(category);
                context.Remove(category);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
