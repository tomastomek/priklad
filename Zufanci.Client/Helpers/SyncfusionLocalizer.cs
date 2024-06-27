using Syncfusion.Blazor;

namespace Zufanci.Client.Helpers
{
    public class SyncfusionLocalizer: ISyncfusionStringLocalizer
    {
        public string GetText(string key)
        {
            return this.ResourceManager.GetString(key);
        }

        public System.Resources.ResourceManager ResourceManager
        {
            get
            {
                return Resources.SfResources.ResourceManager;
            }
        }
    }
}
