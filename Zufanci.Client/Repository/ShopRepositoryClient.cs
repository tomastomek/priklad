using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Shared;

namespace Zufanci.Client.Repository
{
    public class ShopRepositoryClient : IShopRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/shops";

        public ShopRepositoryClient(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task CreateShopAsync(Shop shop)
        {
            var response = await httpService.Post<Shop>(url, shop);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task<bool> DeleteShopAsync(int id)
        {
            var response = await httpService.Delete($"{url}/{id}");
            if (response.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return false;
            }
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return true;
        }

        public async Task<Shop> GetShopAsync(int id)
        {
            var response = await httpService.Get<Shop>($"{url}/{id}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task<List<Shop>> GetShopsAsync()
        {
            var response = await httpService.Get<List<Shop>>(url);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task UpdateShopAsync(Shop shop)
        {
            var response = await httpService.Put<Shop>(url, shop);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}
