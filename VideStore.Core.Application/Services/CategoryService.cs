using AutoMapper;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.Requests.Categories;
using VideStore.Shared.Specifications;

namespace VideStore.Application.Services
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        public async Task<Result<Category>> CreateCategoryAsync(CategoryRequest categoryRequest)
        {
            var category = mapper.Map<Category>(categoryRequest);

            await unitOfWork.Repository<Category>().AddAsync(category);

            var result = await unitOfWork.CompleteAsync();

            return result > 0 ? Result.Success<Category>(category) : Result.Failure<Category>(new Error(500, "Error occured while saving category."));
        }

        public async Task<Result<Category>> GetCategoryByIdAsync(int id)
        {
            var category = await unitOfWork.Repository<Category>().GetEntityAsync(id);
            return category == null ? Result.Failure<Category>(new Error(404, $"Category with id {id} not found")) : Result.Success<Category>(category);
        }

        public async Task<Result<IReadOnlyList<Category>>> GetAllCategoriesAsync()
        {
            var categories = await unitOfWork.Repository<Category>().GetAllAsync();

            return Result.Success<IReadOnlyList<Category>>(categories);
        }

        public async Task<Result<IReadOnlyList<Category>>> SearchAsync(string searchQuery)
        {
            var spec = new BaseSpecifications<Category>() { WhereCriteria = x => x.Name.Contains(searchQuery) };

            var categories = await unitOfWork.Repository<Category>().GetAllAsync(spec);

            return Result.Success<IReadOnlyList<Category>>(categories);
        }

        public async Task<Result<Category>> UpdateCategoryAsync(int id, CategoryRequest categoryRequest)
        {
            var category = await unitOfWork.Repository<Category>().GetEntityAsync(id);
            if (category == null)
                return Result.Failure<Category>(new Error(404, $"Category with id {id} not found."));

            mapper.Map(categoryRequest, category);

            unitOfWork.Repository<Category>().Update(category);

            var result = await unitOfWork.CompleteAsync();

            return result > 0 ? Result.Success<Category>(category) : Result.Failure<Category>(new Error(500, "Error occured while updating category."));
        }

        public async Task<Result<string>> DeleteCategoryAsync(int id)
        {
            var category = await unitOfWork.Repository<Category>().GetEntityAsync(id);

            if (category == null)
                return Result.Failure<string>(new Error(400, $"Category with id {id} not found"));

            unitOfWork.Repository<Category>().Delete(category);

            var result = await unitOfWork.CompleteAsync();

            return result >= 0 ? Result.Success<string>("category delete successfully.") : Result.Failure<string>(new Error(500, "Error occured while deleting category."));
        }
    }
}
