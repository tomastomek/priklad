using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Server.Service;
using Zufanci.Shared;

namespace Zufanci.Server.Repository
{
    public class CategoryRepositoryServer : ICategoryRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly ImageService imageService;

        public CategoryRepositoryServer(IDbContextFactory<ApplicationDbContext> contextFactory, ImageService imageService)
        {
            this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            this.imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        public async Task CreateCategoryAsync(Category category)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Categories.Add(category);
                var entries = await context.SaveChangesAsync();
                if (entries == 0)
                {
                    throw new ApplicationException("[CREATE] Nothing was written to the database");
                }
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                var productWithCategory = await context.Products.Where(x => x.CategoryId == id).ToListAsync();
                if (productWithCategory.Any()) { return false; }
                if (category == null) { return false; }
                await imageService.DeleteImage(category);
                context.Remove(category);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.Categories.OrderBy(x => x.Name).ToListAsync();
            }
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null) { throw new ApplicationException("[GET] Category is null"); }
                return category;
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                await imageService.DeleteImage(category);
                context.Update(category);
                //context.Attach(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
