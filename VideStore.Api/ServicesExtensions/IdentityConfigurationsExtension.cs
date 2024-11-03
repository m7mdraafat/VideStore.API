using Microsoft.AspNetCore.Identity;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Persistence.Context;

namespace VideStore.Api.ServicesExtensions
{
    public static class IdentityConfigurationsExtension
    {
        public static IServiceCollection AddIdentityConfigurations(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;

            }).AddEntityFrameworkStores<StoreDbContext>().AddDefaultTokenProviders();

            return services;
        }
    }
}
