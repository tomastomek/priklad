using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zufanci.Shared;

namespace Zufanci.Server.Service
{
    public class MonitoringDetailsService : IMonitoringDetailsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public MonitoringDetailsService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<List<MonitoringDetail>> GetMonitoringDetails(int Id)
        {
            if (Id > 0)
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    return await context.MonitoringDetails
                        .Include(x => x.MonitoringItem)
                        .Where(x => x.MonitoringItemId == Id)
                        .ToListAsync();
                }
            }
            else
            {
                return new List<MonitoringDetail>();
            }
        }
    }
}
