using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zufanci.Shared
{
    public class MonitoringDetail
    {
        [Key]
        public int Id { get; set; }
        public int MonitoringItemId { get; set; }
        [ForeignKey(nameof(MonitoringItemId))]
        public MonitoringItem? MonitoringItem { get; set; }
        public DateTime? ChangeDetectionTime { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
