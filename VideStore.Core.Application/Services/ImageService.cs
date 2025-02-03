using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using VideStore.Application.Interfaces;

namespace VideStore.Application.Services
{
    public class ImageService(IWebHostEnvironment environment) : IImageService
    {
        public async Task<string> SaveImageAsync(IFormFile file, string folder, string id)
        {
            if (file == null || file.Length == 0)
            {
                return "Invalid image file";
            }

            string singleDirectory = "";
            if (folder.EndsWith("s"))
            {
                singleDirectory = folder.Substring(0, folder.Length - 1);
            }
            else
            {
                singleDirectory = folder;
            }
            var imagePath = Path.Combine("Images", folder, $"{singleDirectory}-{id.ToString()}");
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
            {
                return false; // Invalid image URL
            }

            // Convert the image URL to a file path
            var filePath = Path.Combine(environment.WebRootPath, imageUrl.Replace("/", "\\"));

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true; // Image successfully deleted
                }
                catch (Exception ex)
                {
                    // Log the exception (logging not shown here)
                    return false; // Failed to delete the image
                }
            }



            return false; // File not found
        }

        public async Task<bool> DeleteFolderAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return false; // Invalid folder path
            }

            // Convert the relative folder path to an absolute path
            var directoryPath = Path.Combine(environment.WebRootPath, folderPath.Replace("/", "\\"));

            if (Directory.Exists(directoryPath))
            {
                try
                {
                    // Delete the directory and all its contents
                    Directory.Delete(directoryPath, true); // 'true' parameter ensures deletion of contents
                    return true; // Folder successfully deleted
                }
                catch (Exception ex)
                {
                    // Log the exception (logging not shown here)
                    return false; // Failed to delete the folder
                }
            }

            return false; // Folder not found
        }

    }
}
