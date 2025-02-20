﻿using Google;
using VideStore.Application.Interfaces;
using VideStore.Application.Mapping;
using VideStore.Application.Services;
using VideStore.Domain.Interfaces;
using VideStore.Infrastructure.ExternalServices;
using VideStore.Persistence.Context;
using VideStore.Persistence.Repositories;

namespace VideStore.Api.ServicesExtensions
{
    public static class AddApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenProviderService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();

            // external services

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IGoogleService, GoogleAuthService>();

            // auto mapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly); 
            return services;
        }
    }
}
