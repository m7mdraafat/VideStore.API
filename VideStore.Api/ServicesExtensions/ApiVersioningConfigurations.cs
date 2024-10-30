using Asp.Versioning;

namespace VideStore.Api.ServicesExtensions
{
    public static class ApiVersioningConfigurations
    {
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
              .AddMvc()
              .AddApiExplorer(options =>
              {
                  options.GroupNameFormat = "'v'V";
                  options.SubstituteApiVersionInUrl = true;
              });

            return services;
        }
    }
}
