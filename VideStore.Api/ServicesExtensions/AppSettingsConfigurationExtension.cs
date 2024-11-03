using VideStore.Domain.ConfigurationsData;
namespace VideStore.Api.ServicesExtensions
{
    public static class AppSettingsConfigurationExtension
    {
        public static IServiceCollection ConfigureAppSettingsData(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DatabaseConnections>(configuration.GetSection("ConnectionStrings"));
            services.Configure<MailData>(configuration.GetSection("MailData"));
            services.Configure<HangfireSettingsData>(configuration.GetSection("HangfireSettingsData"));
            services.Configure<GoogleData>(configuration.GetSection("GoogleData"));
            services.Configure<JwtData>(configuration.GetSection("JWT"));

            return services;
        }
    }
}
