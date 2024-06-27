using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Zufanci.Shared
{
    public class MonitoringItem
    {
        public enum MonitoringChange
        {
            NOCHANGE,
            CHANGE,
        }


        public int Id { get; set; }
        [Required(ErrorMessage = "Stránka ke sledování nemůže být prázdná")]
        public string WebsiteUrl { get; set; } = string.Empty;
        [Required(ErrorMessage = "Prvek ke sledování nemůže být prázdný")]
        public string XPathToElement { get; set; } = string.Empty;
        [Required(ErrorMessage = "Je potřeba zadat popis")]
        public string Description { get; set;} = string.Empty;
        public string LastValue { get; set; } = string.Empty;
        public string CurrentValue { get; set; } = string.Empty;
        public int MinuteInterval { get; set; } = 30;
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public MonitoringChange Change { get; set; }
        public int NumChanges { get; set; } = 0;
        public string Password { get; set; } = string.Empty;
        public bool OuterHtml { get; set; } = false;

        [NotMapped]
        [JsonIgnore]
        public Timer? Timer { get; set; }
        public ICollection<MonitoringDetail>? MonitoringDetails { get; set; }
    }
}
