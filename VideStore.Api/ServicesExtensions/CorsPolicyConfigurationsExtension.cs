namespace VideStore.Api.ServicesExtensions
{
    public static class CorsPolicyConfigurationsExtension
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigins", builder =>
                {
                    builder.WithOrigins("https://videstore.vercel.app", "http://localhost:5173")
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });

            });

            return services;
        }
    }
}
