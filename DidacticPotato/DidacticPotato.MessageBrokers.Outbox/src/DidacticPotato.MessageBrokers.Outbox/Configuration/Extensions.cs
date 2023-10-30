using DidacticPotato.MessageBrokers.Outbox.Configuration.Models;
using DidacticPotato.MessageBrokers.Outbox.Processors;
using DidacticPotato.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DidacticPotato.MessageBrokers.Outbox.Configuration;

public static class Extensions
{
    private const string SectionName = "Outbox";

    public static IServiceCollection AddMessageOutbox(this IServiceCollection services, IConfiguration configuration,
        Action<IMessageOutboxConfiguration> configure = null, string sectionName = SectionName)
    {
        services.Configure<OutboxOptions>(configuration.GetSection(sectionName));
        var options = configuration.GetOptions<OutboxOptions>(sectionName);

        var configurator = new MessageOutboxConfiguration(services, options);
        if (configure is null)
        {
            //Tu będzie AddInMemory
        }
        else
        {
            configure(configurator);
        }

        services.AddHostedService<OutboxProcessor>();
        return services;
    }
}