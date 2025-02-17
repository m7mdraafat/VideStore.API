using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json;
using VideStore.Api.ServicesExtensions;
using VideStore.Domain.ConfigurationsData;
using VideStore.Infrastructure;
using VideStore.Persistence;

namespace VideStore.Api
{
    public static class DependencyInjection
    {
        public static WebApplicationBuilder AddApplicationDependencies(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;


            services.ConfigureAppSettingsData(configuration);

            var serviceProvider = services.BuildServiceProvider();

            var databaseConnections = serviceProvider.GetRequiredService<IOptions<DatabaseConnections>>().Value;
            var jwtData = serviceProvider.GetRequiredService<IOptions<JwtData>>().Value;
            var googleData = serviceProvider.GetRequiredService<IOptions<GoogleData>>().Value;
            services.AddControllers();

            services.AddSwaggerServices();

            services.AddApiVersioningConfiguration(); 

            services.AddStoreContext(databaseConnections.StoreConnection);

            services.AddIdentityConfigurations(); // comes before auth service

            services.AddAuthConfigurations(jwtData, googleData);

            services.AddDistributedCache(configuration);

            services.Configure<JsonSerializerOptions>(options =>
            {
                options.PropertyNameCaseInsensitive = true;
                options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.WriteIndented = false;
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            //services.AddRedis(databaseConnections.RedisConnection); 

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("admin", "superAdmin"));
                options.AddPolicy("user", policy => policy.RequireRole("user", "customer"));


            });

            services.AddApplicationServices();

            services.AddHangfireConfigurations(databaseConnections.HangfireConnection);

            services.AddCorsPolicy(); 

            return builder; 


        }
    }
}
