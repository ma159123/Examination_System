using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HotelManagement.Application.Mapping
{
    public static class MappingRegistration
    {
        public static IServiceCollection AddMappings(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;

            // Scan assembly ويلاقي كل IRegister
            config.Scan(Assembly.GetExecutingAssembly());

            // Register Mapper

            services.AddSingleton(config);

            return services;
        }
    }
}
