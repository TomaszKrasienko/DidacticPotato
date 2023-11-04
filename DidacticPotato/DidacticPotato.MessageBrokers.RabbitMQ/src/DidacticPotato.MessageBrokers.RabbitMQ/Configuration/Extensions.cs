using DidacticPotato.MessageBrokers.Publishers;
using DidacticPotato.MessageBrokers.RabbitMQ.Clients;
using DidacticPotato.MessageBrokers.RabbitMQ.Clients.Abstractions;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration.Models;
using DidacticPotato.MessageBrokers.RabbitMQ.Conventions;
using DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;
using DidacticPotato.MessageBrokers.RabbitMQ.Subscribers;
using DidacticPotato.MessageBrokers.Subscribers;
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
        return services
            .SetServices(configuration)
            .SetConnectionFactory(configuration);
    }

    private static IServiceCollection SetServices(this IServiceCollection services, IConfiguration configuration)
    {
        var testOptions = configuration.GetOptions<RabbitMqOptions>(_rabbitSectionName);
        return services
            .Configure<RabbitMqOptions>(configuration.GetSection(_rabbitSectionName))
            .AddSingleton<IBusPublisher, RabbitMqBusPublisher>()
            .AddSingleton<IRabbitMqClient, RabbitMqClient>()
            .AddSingleton<IConventionsRegistry>(opt =>
            {
                var options = configuration.GetOptions<RabbitMqOptions>(_rabbitSectionName);
                var conventionsRegistry = new RabbitMqConventionsRegistry();
                foreach (var routing in options.Routing)
                {
                    var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var type = allAssemblies.FirstOrDefault(x => x.GetType(routing.Type) != null)
                        ?.GetType(routing.Type);
                    if (type is not null)
                    {
                        conventionsRegistry. Add(
                            type,
                            new MessageConvention(routing.Exchange.Name, routing.Exchange.Type,
                                routing.Exchange.Durable,
                                routing.Exchange.AutoDelete, routing.Exchange.Arguments, routing.RoutingKey,
                                routing.Queue.Name, routing.Queue.Durable, routing.Queue.Exclusive,
                                routing.Queue.AutoDelete, routing.Queue.Arguments));
                    }
                }

                return conventionsRegistry;
            })
            .AddSingleton<IConventionsProvider, RabbitMqConventionsProvider>()
            .AddSingleton<MessageSubscriberChannel>()
            .AddSingleton<IBusSubscriber, RabbitMqBusSubscriber>()
            .AddHostedService<RabbitMqBackgroundService>();
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