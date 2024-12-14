using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VideStore.Domain.ConfigurationsData;
using VideStore.Infrastructure.Utilities;

namespace VideStore.Infrastructure;
public static class ServiceExtensions
{
    public static IServiceCollection AddHangfireConfigurations(this IServiceCollection services, string connectionString)
    {
        services.AddHangfire(x => x.UseSqlServerStorage(connectionString));

        services.AddHangfireServer(); 


        return services;
    }


    public static WebApplication UseHangfireDashboardAndRecurringJob(this WebApplication app, IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var hangfireData = serviceProvider.GetRequiredService<IOptions<HangfireSettingsData>>().Value;

        app.UseHangfireDashboard(hangfireData.DashboardUrl, new DashboardOptions()
        {
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter()
                {
                    User = hangfireData.UserName,
                    Pass = hangfireData.Password
                }
            },
            DashboardTitle = hangfireData.ServerName,
        });

        // Schedule recurring job
        RecurringJob.AddOrUpdate<DataDeletionJob>("delete-inactive-identity-codes",
            x => x.DeleteExpiredIdentityCodes(),
            Cron.Daily(1));

        RecurringJob.AddOrUpdate<DataDeletionJob>("delete-inactive-refresh-tokens",
            x => x.DeleteExpiredOrRevokedRefreshTokens(),
            Cron.Hourly(1));


        return app;
    }


}