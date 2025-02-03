
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Requests.Categories;

namespace VideStore.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<Category>> CreateCategoryAsync(CategoryRequest request);
        Task<Result<Category>> GetCategoryByIdAsync(string id);
        Task<Result<IReadOnlyList<Category>>> GetAllCategoriesAsync();
        Task<Result<IReadOnlyList<Category>>> SearchAsync(string searchQuery);
        Task<Result<Category>> UpdateCategoryAsync(string id, CategoryRequest request);
        Task<Result<string>> DeleteCategoryAsync(string id);

    }
}
