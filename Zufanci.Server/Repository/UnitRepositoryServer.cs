using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Shared;

namespace Zufanci.Server.Repository
{
    public class UnitRepositoryServer : IUnitRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public UnitRepositoryServer(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task CreateUnitAsync(Unit unit)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Units.Add(unit);
                var entries = await context.SaveChangesAsync();
                if (entries == 0)
                {
                    throw new ApplicationException("Nothing was written to the database");
                }
            }
        }

        public async Task<bool> DeleteUnitAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var unit = await context.Units.FirstOrDefaultAsync(x => x.Id == id);
                var productPrice = await context.ProductPrices.Where(x => x.UnitId == id).ToListAsync();
                if (productPrice.Any()) { return false; }
                if (unit == null) { return false; }
                context.Remove(unit);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Unit> GetUnitAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var unit = await context.Units.FirstOrDefaultAsync(x => x.Id == id);
                if (unit == null) { throw new ApplicationException("Unit is null"); }
                return unit;
            }
        }

        public async Task<List<Unit>> GetUnitsAsync()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.Units.ToListAsync();
            }
        }

        public async Task UpdateUnitAsync(Unit unit)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                context.Update(unit);
                var entries = await context.SaveChangesAsync();
                if (entries == 0)
                {
                    throw new ApplicationException("Cannot update record in database");
                }
            }
        }
    }
}
