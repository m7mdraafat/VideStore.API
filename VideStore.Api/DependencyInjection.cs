using Microsoft.Extensions.Options;
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

            services.AddApplicationServices();

            services.AddHangfireConfigurations(databaseConnections.HangfireConnection);

            services.AddCorsPolicy(); 

            return builder; 


        }
    }
}
