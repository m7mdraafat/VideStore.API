using VideStore.Application.Interfaces;
using VideStore.Application.Services;
using VideStore.Domain.Interfaces;
using VideStore.Infrastructure.ExternalServices;
using VideStore.Infrastructure.Interfaces;
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
            // external services

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IGoogleService, GoogleAuthService>();

            return services;
        }
    }
}
