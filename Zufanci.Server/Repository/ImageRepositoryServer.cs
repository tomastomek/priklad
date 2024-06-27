using Zufanci.Client.Repository.IRepository;
using Zufanci.Server.Service;

namespace Zufanci.Server.Repository
{
    public class ImageRepositoryServer : IImageRepository
    {
        private readonly IFileUpload fileUpload;

        public ImageRepositoryServer(IFileUpload fileUpload)
        {
            this.fileUpload = fileUpload ?? throw new ArgumentNullException(nameof(fileUpload));
        }

        public async Task<string> CreateImageAsync(string imageBase64, string location)
        {
            try
            {
                string imagePath = fileUpload.UploadFile(imageBase64, location);
                return imagePath;
            }
            catch (Exception e) 
            {
                return await Task.FromResult(e.Message);
            }
        }

        public async Task DeleteImageAsync(string imageName, string location)
        {
            bool result = await Task.FromResult(fileUpload.DeleteFile(imageName, location));
            if (!result) { throw new ApplicationException("There was an error deleting an image."); }
        }
    }
}
