using Microsoft.AspNetCore.Http;

namespace VideStore.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile file, string folder, string id);
        Task<bool> DeleteImageAsync(string imageUrl);
        Task<bool> DeleteFolderAsync(string folderPath);

    }
}
