
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FSAKEB.Application.Extensions
{
    public static class AutoMapperConfiguration
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                var entryAssembly = Assembly.GetEntryAssembly();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                services.AddAutoMapper(configuration => { configuration.AddExpressionMapping(); }, executingAssembly,
                    entryAssembly);

                return services;
            }
        }

    }
}
