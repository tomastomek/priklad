using Microsoft.EntityFrameworkCore;
using Zufanci.Shared;

namespace Zufanci.Server.Service
{
    /// <summary>
    /// Service for managing images in the application.
    /// </summary>
    public class ImageService
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly IFileUpload fileUpload;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService"/> class.
        /// </summary>
        /// <param name="contextFactory">The factory to create instances of the <see cref="ApplicationDbContext"/>.</param>
        /// <param name="fileUpload">The <see cref="IFileUpload"/> instance.</param>
        public ImageService(IDbContextFactory<ApplicationDbContext> contextFactory, IFileUpload fileUpload)
        {
            this.contextFactory = contextFactory;
            this.fileUpload = fileUpload;
        }

        /// <summary>
        /// Deletes the image associated with the specified entity.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="entity">The entity whose image will be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of entity is not supported.</exception>
        public async Task DeleteImage<T>(T entity) where T : class
        {
            string imageNameProperty = typeof(T) switch
            {
                Type t when t == typeof(Category) => "CategoryImage",
                Type t when t == typeof(Product) => "ImageName",
                Type t when t == typeof(Shop) => "ShopImage",
                _ => throw new ArgumentException("Unsupported type"),
            };

            using (var context = contextFactory.CreateDbContext())
            {
                var currentId = entity.GetType().GetProperty("Id").GetValue(entity);
                var currentImage = await context.Set<T>()
                    .Where(entityType => EF.Property<object>(entityType, "Id").Equals(currentId))
                    .Select(entityType => EF.Property<string>(entityType, imageNameProperty))
                    .FirstOrDefaultAsync();

                if (currentImage != null)
                {
                    var substringLength = typeof(T) switch
                    {
                        Type t when t == typeof(Category) => 17,
                        Type t when t == typeof(Product) => 16,
                        Type t when t == typeof(Shop) => 13,
                        _ => throw new ArgumentException("Unsupported type"),
                    };

                    fileUpload.DeleteFile(currentImage.Substring(substringLength), entity.GetType().Name);
                }
            }
        }    
    }
}
