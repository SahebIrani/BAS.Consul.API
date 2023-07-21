using Consul;

namespace BAS.Consul.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsulConfig(this IServiceCollection services, string configKey)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton<IConsulClient>(consul => new ConsulClient(consulConfig =>
        {
            consulConfig.Address = new Uri(configKey);
        }));

        return services;
    }

    public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            //consul address  
            var address = configuration["Consul:Host"];
            consulConfig.Address = new Uri(address!);
        }, null, handlerOverride =>
        {
            //disable proxy of httpclienthandler  
            handlerOverride.Proxy = null;
            handlerOverride.UseProxy = false;
        }));
        return services;
    }
}