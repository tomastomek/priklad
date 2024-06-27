using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Shared;

namespace Zufanci.Client.Repository
{
    public class CategoryRepositoryClient : ICategoryRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/categories";

        public CategoryRepositoryClient(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<Category> GetCategoryAsync(int Id)
        {
            var response = await httpService.Get<Category>($"{url}/{Id}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await httpService.Get<List<Category>>(url);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task CreateCategoryAsync(Category category)
        {
            var response = await httpService.Post<Category>(url, category);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var response = await httpService.Put<Category>(url, category);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task<bool> DeleteCategoryAsync(int Id)
        {
            var response = await httpService.Delete($"{url}/{Id}");
            if (response.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Conflict)
            { return false; }
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return true;
        }
    }
}
