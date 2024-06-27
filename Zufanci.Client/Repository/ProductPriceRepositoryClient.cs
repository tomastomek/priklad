using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Shared;

namespace Zufanci.Client.Repository
{
    public class ProductPriceRepositoryClient : IProductPriceRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/prices";

        public ProductPriceRepositoryClient(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task CreatePricesAsync(ProductPrice productPrice)
        {
            var response = await httpService.Post(url, productPrice);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task DeletePricesAsync(int id)
        {
            var response = await httpService.Delete($"{url}/{id}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task<List<ProductPrice>> GetPricesAsync(int? id = null)
        {
            var response = await httpService.Get<List<ProductPrice>>($"{url}/{id}");
            if (response == null)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task<List<ProductPrice>> GetAllPricesAsync()
        {
            var response = await httpService.Get<List<ProductPrice>>($"{url}");
            if (response == null)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task UpdatePricesAsync(ProductPrice productPrice)
        {
            var response = await httpService.Put(url, productPrice);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}
