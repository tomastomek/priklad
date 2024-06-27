using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using Zufanci.Server.Helpers;
using Zufanci.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Zufanci.Server.Service
{
    /// <summary>
    /// Service for monitoring items and performing actions based on monitoring results.
    /// </summary>
    public class MonitoringService : IMonitoringService
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly IDistributedCache cache;
        private readonly Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
        private readonly EmailSettings emailSettings;
        private readonly IHubContext<MonitoringChangeHub> hubContext;
        private readonly IStringParserService parser = new StringParserService();

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoringService"/> class.
        /// </summary>
        /// <param name="contextFactory">The factory to create instances of the <see cref="ApplicationDbContext"/>.</param>
        /// <param name="cache">The distributed cache.</param>
        /// <param name="emailSettings">The email settings.</param>
        /// <param name="hubContext">Monitoring change hub context.</param>
        public MonitoringService(IDbContextFactory<ApplicationDbContext> contextFactory, IDistributedCache cache, IOptions<EmailSettings> emailSettings, IHubContext<MonitoringChangeHub> hubContext)
        {
            this.contextFactory = contextFactory;
            this.cache = cache;
            this.emailSettings = emailSettings.Value;
            this.hubContext = hubContext;
        }

        /// <summary>
        /// Retrieves a monitoring item asynchronously.
        /// </summary>
        /// <param name="itemId">The ID of the monitoring item to retrieve.</param>
        /// <returns>The retrieved monitoring item.</returns>
        public async Task<MonitoringItem> GetMonitoringItemAsync(int itemId)
        {
            // Try to get the item from the cache
            var cachedItem = await GetCachedMonitoringItemAsync(itemId);
            if (cachedItem != null)
            {
                return cachedItem;
            }

            using (var context = contextFactory.CreateDbContext())
            {
                // If not found in the cache, retrieve it from the database
                var item = await context.MonitoringItems.FindAsync(itemId);
                if (item != null)
                {
                    // Store retrieved item in the cache
                    await CacheMonitoringItemAsync(item);

                    // Start timer for the retrieved item
                    StartTimerForMonitoringItem(item);
                }

                return item;
            }
        }

        /// <summary>
        /// Retrieves all monitoring items asynchronously.
        /// </summary>
        /// <returns>A list of all monitoring items.</returns>
        public async Task<List<MonitoringItem>> GetMonitoringItemsAsync()
        {
            // Try to get the monitoring items from the cache
            var cachedItemsJson = await cache.GetStringAsync("MonitoringItems");
            if (!string.IsNullOrEmpty(cachedItemsJson))
            {
                var cachedItems = JsonConvert.DeserializeObject<List<MonitoringItem>>(cachedItemsJson);
                return cachedItems;
            }

            using (var context = contextFactory.CreateDbContext())
            {
                // If not found in the cache, retrieve all items from the database
                var items = await context.MonitoringItems.ToListAsync();

                // Store the retrieved items in the cache
                await cache.SetStringAsync("MonitoringItems", JsonConvert.SerializeObject(items));

                // Start timers for all retrieved items
                foreach (var item in items)
                {
                    StartTimerForMonitoringItem(item);
                }

                return items;
            }
        }

        /// <summary>
        /// Adds a new monitoring item asynchronously.
        /// </summary>
        /// <param name="item">The monitoring item to add.</param>
        /// <returns>The ID of the added monitoring item.</returns>
        public async Task<int> AddMonitoringItemAsync(MonitoringItem item)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                await context.MonitoringItems.AddAsync(item);
                await context.SaveChangesAsync();

                await CacheMonitoringItemAsync(item);
                StartTimerForMonitoringItem(item);

                return item.Id;
            }
        }

        /// <summary>
        /// Removes a monitoring item asynchronously.
        /// </summary>
        /// <param name="itemId">The ID of the monitoring item to remove.</param>
        /// <returns>True if the item is removed successfully; otherwise, false.</returns>
        public async Task<bool> RemoveMonitoringItemAsync(int itemId)
        {
            try
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    var item = await context.MonitoringItems.FindAsync(itemId);
                    if (item == null)
                    {
                        return false;
                    }

                    context.MonitoringItems.Remove(item);
                    await context.SaveChangesAsync();

                    await RemoveMonitoringItemFromCacheAsync(itemId);

                    StopTimerForMonitoringItem(itemId);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing monitoring item: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves a monitoring item from the cache asynchronously.
        /// </summary>
        /// <param name="itemId">The ID of the monitoring item to retrieve from the cache.</param>
        /// <returns>The cached monitoring item if found; otherwise, null.</returns>
        private async Task<MonitoringItem> GetCachedMonitoringItemAsync(int itemId)
        {
            var cachedItemsJson = await cache.GetStringAsync("MonitoringItems");
            if (string.IsNullOrEmpty(cachedItemsJson))
            {
                return null;
            }

            var cachedItems = JsonConvert.DeserializeObject<List<MonitoringItem>>(cachedItemsJson);
            return cachedItems.FirstOrDefault(item => item.Id == itemId);
        }

        /// <summary>
        /// Adds a monitoring item to the cache asynchronously.
        /// </summary>
        /// <param name="item">The monitoring item to cache.</param>
        private async Task CacheMonitoringItemAsync(MonitoringItem item)
        {
            var cachedItemsJson = await cache.GetStringAsync("MonitoringItems");
            var cachedItems = string.IsNullOrEmpty(cachedItemsJson)
                ? new List<MonitoringItem>()
                : JsonConvert.DeserializeObject<List<MonitoringItem>>(cachedItemsJson);

            // Add the new item to the cached list
            cachedItems?.Add(item);

            // Update the cache
            await cache.SetStringAsync("MonitoringItems", JsonConvert.SerializeObject(cachedItems));
        }

        /// <summary>
        /// Updates a monitoring item in the cache asynchronously.
        /// </summary>
        /// <param name="item">The monitoring item to update in the cache.</param>
        /// <returns>True if the item is updated in the cache successfully; otherwise, false.</returns>
        private async Task<bool> UpdateCachedMonitoringItemAsync(MonitoringItem item)
        {
            try
            {
                var cachedItemsJson = await cache.GetStringAsync("MonitoringItems");

                if (string.IsNullOrEmpty(cachedItemsJson))
                {
                    return false; // Cache is empty
                }

                var cachedItems = JsonConvert.DeserializeObject<List<MonitoringItem>>(cachedItemsJson);

                var itemToUpdate = cachedItems?.FirstOrDefault(m => m.Id == item.Id);
                if (itemToUpdate != null)
                {
                    // Update the item in the cache
                    itemToUpdate.Change = item.Change;
                    itemToUpdate.NumChanges = item.NumChanges;

                    // Update the cache
                    await cache.SetStringAsync("MonitoringItems", JsonConvert.SerializeObject(cachedItems));
                    return true; // Update successful
                }

                return false; // Item not found in cache
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating monitoring item in cache: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Removes a monitoring item from the cache asynchronously.
        /// </summary>
        /// <param name="itemId">The ID of the monitoring item to remove from the cache.</param>
        /// <returns>True if the item is removed from the cache successfully; otherwise, false.</returns>
        private async Task<bool> RemoveMonitoringItemFromCacheAsync(int itemId)
        {
            try
            {
                var cachedItemsJson = await cache.GetStringAsync("MonitoringItems");

                if (string.IsNullOrEmpty(cachedItemsJson))
                {
                    return false; // Cache is empty
                }

                var cachedItems = JsonConvert.DeserializeObject<List<MonitoringItem>>(cachedItemsJson);

                var itemToRemove = cachedItems?.FirstOrDefault(item => item.Id == itemId);
                if (itemToRemove != null)
                {
                    cachedItems?.Remove(itemToRemove);
                    await cache.SetStringAsync("MonitoringItems", JsonConvert.SerializeObject(cachedItems));
                    return true; // Removal successful
                }

                return false; // Item not found in cache
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing monitoring item from cache: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Starts a timer for monitoring a specific item.
        /// </summary>
        /// <param name="item">The monitoring item to start the timer for.</param>
        private void StartTimerForMonitoringItem(MonitoringItem item)
        {
            // Check if timer already exists for the item
            if (!timers.ContainsKey(item.Id))
            {
                TimerCallback timerCallback = async _ =>
                {
                    // Perform monitoring logic
                    await CheckMonitoredElementAsync(item);
                };

                // Start the timer
                var timer = new Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(item.MinuteInterval));
                timers[item.Id] = timer;
            }
        }

        /// <summary>
        /// Stops the timer for a monitoring item.
        /// </summary>
        /// <param name="itemId">The ID of the monitoring item.</param>
        private void StopTimerForMonitoringItem(int itemId)
        {
            if (timers.ContainsKey(itemId))
            {
                // Stop and dispose the timer for the item
                timers[itemId].Change(Timeout.Infinite, Timeout.Infinite);
                timers[itemId].Dispose();
                timers.Remove(itemId);
            }
        }

        /// <summary>
        /// Checks a monitored element asynchronously.
        /// </summary>
        /// <param name="item">The monitoring item containing information about the element to check.</param>
        private async Task CheckMonitoredElementAsync(MonitoringItem item)
        {
            // Logic to check the monitored element and determine if conditions are met
            // Return true if conditions are met, false otherwise
            try
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    var httpClient = new HttpClient();
                    var html = await httpClient.GetStringAsync(item.WebsiteUrl);
                    var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    var element = htmlDocument.DocumentNode.SelectSingleNode(item.XPathToElement);
                    string? value = null;
                    if (item.OuterHtml)
                    {
                        value = element?.OuterHtml;
                        value = value.Substring(1, value.Length - 2);
                    }
                    else
                    {
                        value = element?.InnerHtml;
                    }
                    if (!string.IsNullOrEmpty(item.Template))
                    {
                        value = parser.ParseString(value, item.Template);
                    }
                    Console.WriteLine($"[INFO] {DateTime.Now} - Checking element {value} on website {item.WebsiteUrl} current value: {value} for id {item.Id}");
                    if (!string.IsNullOrEmpty(item.XPathToElement) && value != item.CurrentValue)
                    {                        
                        Console.WriteLine($"Last value is: {item.LastValue} | new value is: {value} | Time checked: {item.LastUpdated}");
                        Console.WriteLine($"[INFO] Sending email with change");
                        await SendEmailAsync(item.Description, item.CurrentValue, value, DateTime.Now.ToString(), item.WebsiteUrl, item.EmailAddress);
                        item.LastValue = item.CurrentValue;
                        item.CurrentValue = value;
                        item.LastUpdated = DateTime.Now;
                        item.Change = MonitoringItem.MonitoringChange.CHANGE;
                        item.NumChanges++;
                        context.MonitoringItems.Update(item);

                        MonitoringDetail detail = new MonitoringDetail
                        {
                            ChangeDetectionTime = DateTime.Now,
                            Value = value,
                            MonitoringItemId = item.Id,
                        };
                        context.MonitoringDetails.Add(detail);
                        await context.SaveChangesAsync();
                        await UpdateCachedMonitoringItemAsync(item);
                        await hubContext.Clients.All.SendAsync("UpdateUI", item.Id, item.NumChanges);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends an email notification asynchronously.
        /// </summary>
        /// <param name="description">The description of the monitored element.</param>
        /// <param name="previousValue">The previous value of the monitored element.</param>
        /// <param name="newValue">The new value of the monitored element.</param>
        /// <param name="changeDetectionTime">The time when the change was detected.</param>
        /// <param name="address">The URL of the monitored website.</param>
        /// <param name="email">The email address to send the notification to.</param>
        /// <param name="pattern">The pattern for converting the value.</param>
        public async Task SendEmailAsync(string description, string previousValue, string newValue, string changeDetectionTime, string address, string email, string pattern = "")
        {
            if (string.IsNullOrEmpty(previousValue)) { return; }
            try
            {
                using (var smtpClient = new SmtpClient(emailSettings.Host))
                {
                    smtpClient.Port = emailSettings.Port;
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(emailSettings.EmailAddress, emailSettings.EmailPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailSettings.EmailAddress, "Váš služebníček"),
                        Subject = $"📢 Byla zjištěna změna u: {description}",
                        IsBodyHtml = true, // Set to true to use HTML formatting
                        Body = $@"
                    <p>Ve sledovaném prvku došlo ke změně:</p>
                    <ul>
                        <li><b>Stránka:</b> {address}</li>
                        <li><b>Předchozí hodnota:</b> {previousValue}</li>
                        <li><b>Nová hodnota:</b> {newValue}</li>
                        <li><b>Čas detekce:</b> {changeDetectionTime}</li>
                    </ul>
                    <p>S pozdravem,</p>
                    <p><br/>Váš služebníček</p>"
                    };
                    mailMessage.To.Add(email);

                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine("Email sent successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
