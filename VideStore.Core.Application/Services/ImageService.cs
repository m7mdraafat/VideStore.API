using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using VideStore.Application.Interfaces;

namespace VideStore.Application.Services
{
    public class ImageService(IWebHostEnvironment environment) : IImageService
    {
        public async Task<string> SaveImageAsync(IFormFile file, string folder, int id)
        {
            if (file == null || file.Length == 0)
            {
                return "Invalid image file";
            }

            if (folder.EndsWith("s"))
            {
                folder = folder.Substring(0, folder.Length - 1);
            }
            var imagePath = Path.Combine("Images", folder, $"{folder}-{id.ToString()}");
            var finalPath = Path.Combine(environment.WebRootPath, imagePath);
            if (!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
            }


            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(finalPath, fileName);

            while (File.Exists(filePath))
            {
                 fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); 
                 filePath = Path.Combine(finalPath, fileName);
            }
            // save the file to the specific path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = Path.Combine(imagePath, fileName).Replace("\\", "/");

            return imageUrl;

        }
        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            // Construct the full path to the image file
            var imagePath = Path.Combine(environment.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath); // Delete the file
                return true; // Return success
            }

            return false; // Return failure if the file doesn't exist
        }
    }
}
