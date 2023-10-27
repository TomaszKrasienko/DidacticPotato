using DidacticPotato.MessageBrokers.Publishers;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration.Models;
using DidacticPotato.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Configuration;

public static class Extensions
{
    private const string _rabbitSectionName = "RabbitMq";
    
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

    private static IServiceCollection SetConnectionFactory(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetOptions<RabbitMqOptions>(_rabbitSectionName);
        var connectionFactory = new ConnectionFactory()
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            RequestedHeartbeat = options.RequestedHeartbeat,
            RequestedConnectionTimeout = options.RequestedConnectionTimeout,
            RequestedChannelMax = options.RequestedChannelMax,
            Ssl = options.Ssl is null ? 
                new SslOption() : 
                new SslOption(options.Ssl.ServerName, options.Ssl.CertificatePath, options.Ssl.Enabled)
        };

        var producerConnection = connectionFactory.CreateConnection($"{options.ConnectionName}_producer");
        var consumerConnection = connectionFactory.CreateConnection($"{options.ConnectionName}_consumer");

        services.AddSingleton(new ProducerConnection(producerConnection));        
        services.AddSingleton(new ConsumerConnection(consumerConnection));
        return services;
    }
}