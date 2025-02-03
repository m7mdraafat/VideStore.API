using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Requests.Categories;
using VideStore.Shared.Specifications;

namespace VideStore.Application.Services
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService) : ICategoryService
    {
        public async Task<Result<Category>> CreateCategoryAsync(CategoryRequest categoryRequest)
        {
            // Map the request to the Category entity
            var category = mapper.Map<Category>(categoryRequest);

            // Add the new category to the repository and save it to generate the Id
            category.CoverImageUrl = "empty";
            await unitOfWork.Repository<Category>().AddAsync(category);
            var result = await unitOfWork.CompleteAsync();

            if (result > 0)
            {
                // Save the image after the Id is generated
                if (categoryRequest.Image != null)
                {
                    var folderType = "Category";
                    var imageUrl = await imageService.SaveImageAsync(categoryRequest.Image, folderType, category.Id);
                    category.CoverImageUrl = imageUrl;

                    // Update the category with the new image URL
                    unitOfWork.Repository<Category>().Update(category);
                    await unitOfWork.CompleteAsync();
                }

                return Result.Success<Category>(category);
            }

            return Result.Failure<Category>(new Error(500, "Error occurred while saving category."));
        }
        public async Task<Result<Category>> GetCategoryByIdAsync(string id)
        {
            var spec = new BaseSpecifications<Category>
            {
                WhereCriteria = c => c.Id == id,

            };
            spec.Includes.Add(q => q.Include(c => c.Products)
                .ThenInclude(p => p.ProductImages));
            spec.Includes.Add(q => q.Include(c => c.Products)
                .ThenInclude(p => p.Color));
            spec.Includes.Add(q => q.Include(c => c.Products)
                .ThenInclude(p => p.ProductSizes));
            spec.Includes.Add(q => q.Include(c => c.Products)
                .ThenInclude(p => p.ProductSizes).ThenInclude(ps=>ps.Size));


            var category = await unitOfWork.Repository<Category>().GetEntityAsync(spec);
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

        public async Task<Result<Category>> UpdateCategoryAsync(string id, CategoryRequest categoryRequest)
        {
            var category = await unitOfWork.Repository<Category>().GetEntityAsync(id);
            if (category == null)
                return Result.Failure<Category>(new Error(404, $"Category with id {id} not found."));

            // Save the new image if provided
            if (categoryRequest.Image != null)
            {
                // Attempt to delete the old image file if it exists
                if (!string.IsNullOrEmpty(category.CoverImageUrl))
                {
                    var imageDeleted = await imageService.DeleteImageAsync(category.CoverImageUrl);
                    if (!imageDeleted)
                    {
                        return Result.Failure<Category>(new Error(500, "Error occurred while deleting the old category image."));

                    }
                }

                // Save the new image
                var folderType = "Category";
                var newImageUrl = await imageService.SaveImageAsync(categoryRequest.Image, folderType, id);
                category.CoverImageUrl = newImageUrl; // Update the CoverImageUrl with the new image URL
            }

            // Map other properties
            mapper.Map(categoryRequest, category);

            unitOfWork.Repository<Category>().Update(category);

            var result = await unitOfWork.CompleteAsync();

            return result > 0 ? Result.Success<Category>(category) : Result.Failure<Category>(new Error(500, "Error occurred while updating category."));
        }


        public async Task<Result<string>> DeleteCategoryAsync(string id)
        {
            var category = await unitOfWork.Repository<Category>().GetEntityAsync(id);

            if (category == null)
                return Result.Failure<string>(new Error(404, $"Category with id {id} not found"));

            if (!string.IsNullOrEmpty(category.CoverImageUrl))
            {
                var imageDeleted = await imageService.DeleteFolderAsync($"Images/Category/Category-{category.Id}");
                if (!imageDeleted)
                {
                    Console.WriteLine($"category with category id {category.Id} does not have image.");
                }
            }

            unitOfWork.Repository<Category>().Delete(category);

            var result = await unitOfWork.CompleteAsync();

            return result >= 0 ? Result.Success<string>("category delete successfully.") :
                Result.Failure<string>(new Error(500, "Error occured while deleting category."));
        }
    }
}
