﻿using Microsoft.AspNetCore.Http;

namespace VideStore.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile file, string folder, int id);
    }
}