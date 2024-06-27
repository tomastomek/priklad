using Zufanci.Shared;

namespace Zufanci.Server.Service
{
    public interface IMonitoringService
    {
        Task<int> AddMonitoringItemAsync(MonitoringItem itemDto);
        Task<bool> RemoveMonitoringItemAsync(int itemId);
        Task<List<MonitoringItem>> GetMonitoringItemsAsync();
    }
}
