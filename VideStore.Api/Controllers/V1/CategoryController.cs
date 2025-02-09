﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideStore.Api.Extensions;
using VideStore.Application.Interfaces;
using VideStore.Shared.DTOs.Requests.Categories;

namespace VideStore.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            var result = await categoryService.GetAllCategoriesAsync();

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(string id)
        {
            var result = await categoryService.GetCategoryByIdAsync(id);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateAsync([FromForm] CategoryRequest categoryRequest)
        {
            var result = await categoryService.CreateCategoryAsync(categoryRequest);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateAsync(string id, [FromForm]CategoryRequest categoryRequest)
        {
            var result = await categoryService.UpdateCategoryAsync(id, categoryRequest);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var result = await categoryService.DeleteCategoryAsync(id);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        [HttpGet("{searchQuery:alpha}")]
        public async Task<ActionResult> SearchAsync(string searchQuery)
        {
            var result = await categoryService.SearchAsync(searchQuery);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }
    }
}
