using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FSAKEB.Application.Extensions
{
    public static class HangfireConfiguration
    {
        public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddHangfire(config =>
            //{
            //    config.UseSqlServerStorage(configuration.GetConnectionString("AppConnection"));
            //});

            //services.AddHangfireServer();

            return services;
        }
    }
}
