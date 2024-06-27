using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Shared;

namespace Zufanci.Client.Repository
{
    public class ProductRepositoryClient : IProductRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/products";

        public ProductRepositoryClient(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task CreateProductAsync(Product product)
        {
            var response = await httpService.Post<Product>(url, product);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var response = await httpService.Delete($"{url}/{id}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var response = await httpService.Get<Product>($"{url}/{id}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await httpService.Get<List<Product>>(url);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task UpdateProductAsync(Product product)
        {
            var response = await httpService.Put<Product>(url, product);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}
