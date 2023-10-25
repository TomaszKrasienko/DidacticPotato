using DidacticPotato.MessageBrokers.Publishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Configuration;

public static class Extensions
{
    public static IServiceCollection SetRabbitMqConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }

    private static IServiceCollection SetServices(this IServiceCollection services)
    {
        return 
            services.AddSingleton<IBusPublisher, RabbitMqBusPublisher>();
    }
}