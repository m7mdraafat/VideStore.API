
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.Requests.Categories;

namespace VideStore.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<Category>> CreateCategoryAsync(CategoryRequest request);
        Task<Result<Category>> GetCategoryByIdAsync(int id);
        Task<Result<IReadOnlyList<Category>>> GetAllCategoriesAsync();
        Task<Result<IReadOnlyList<Category>>> SearchAsync(string searchQuery);
        Task<Result<Category>> UpdateCategoryAsync(int id, CategoryRequest request);
        Task<Result<string>> DeleteCategoryAsync(int id);

    }
}
