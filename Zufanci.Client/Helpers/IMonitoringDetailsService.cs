using Zufanci.Shared;

namespace Zufanci.Server.Service
{
    public interface IMonitoringDetailsService
    {
        Task<List<MonitoringDetail>> GetMonitoringDetails(int id);
    }
}
