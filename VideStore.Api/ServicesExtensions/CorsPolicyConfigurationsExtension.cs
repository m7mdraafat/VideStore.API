namespace VideStore.Api.ServicesExtensions
{
    public static class CorsPolicyConfigurationsExtension
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigins",
                    builder => builder.AllowAnyOrigin() // Allow all origins
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            return services;
        }
    }
}
