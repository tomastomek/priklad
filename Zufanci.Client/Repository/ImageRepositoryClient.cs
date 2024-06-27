using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;

namespace Zufanci.Client.Repository
{
    public class ImageRepositoryClient : IImageRepository
    {
        private readonly IHttpService httpService;
        private readonly string url = "api/images";

        public ImageRepositoryClient(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<string> CreateImageAsync(string imageBase64, string location)
        {
            var response = await httpService.Post($"{url}/{location}", imageBase64);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return await response.GetBody();
        }

        public async Task DeleteImageAsync(string imageName, string location)
        {
            var response = await httpService.Delete($"{url}/{location}/{imageName}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}
