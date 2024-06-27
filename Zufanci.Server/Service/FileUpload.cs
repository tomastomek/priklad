namespace Zufanci.Server.Service
{
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUpload"/> class.
        /// </summary>
        /// <param name="webHostEnvironment">The hosting environment.</param>
        public FileUpload(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Uploads a file to the specified location.
        /// </summary>
        /// <param name="imageBase64">The base64 encoded image data.</param>
        /// <param name="location">The location to save the file.</param>
        /// <returns>The path to the uploaded file.</returns>
        public string UploadFile(string imageBase64, string location)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString() + GetFileExtension(imageBase64);
                string directory = string.Empty;

                // Pokud content root path obsahuje app, tak aplikace běží na Linuxu (Docker)
                if (webHostEnvironment.ContentRootPath != "/app/")
                    directory = $"{webHostEnvironment.WebRootPath}\\images\\{location}";
                directory = $"wwwroot/images/{location}";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string filePath = Path.Combine(directory, fileName);

                byte[] imageBytes = Convert.FromBase64String(imageBase64);
                File.WriteAllBytes(filePath, imageBytes);

                //await using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                //await file.OpenReadStream(512000*4).CopyToAsync(fileStream);

                string fullPath = $"/images/{location}/{fileName}";
                return fullPath;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes a file from the specified location.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <param name="location">The location of the file.</param>
        /// <returns>True if the file is deleted successfully; otherwise, false.</returns>
        public bool DeleteFile(string fileName, string location)
        {
            string filePath = string.Empty;
            if (webHostEnvironment.ContentRootPath != "/app/")
                filePath = $"{webHostEnvironment.WebRootPath}\\images\\{location}/{fileName}";
            filePath = $"wwwroot/images/{location}/{fileName}";
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Determines the file extension based on the provided base64 string.
        /// </summary>
        /// <param name="base64String">The base64 encoded image data.</param>
        /// <returns>The file extension corresponding to the image format.</returns>
        private string GetFileExtension(string base64String)
        {
            string data = base64String.Substring(0, 5).ToUpper();
            if (data == "IVBOR")
            {
                return ".png";
            }
            if (data == "/9J/4")
            {
                return ".jpeg";
            }
            return "";
        }
    }
}
