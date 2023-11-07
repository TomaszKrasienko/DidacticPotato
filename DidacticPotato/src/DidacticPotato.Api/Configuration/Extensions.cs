using System;
using DidacticPotato.Api.Configuration.Models;
using DidacticPotato.Api.MongoDocuments;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration;
using DidacticPotato.Options;
using DidacticPotato.Persistence.MongoDB.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DidacticPotato.Api.Configuration;

public static class Extensions
{
    public static IServiceCollection SetConfiguration(
        this IServiceCollection services, IConfiguration configuration)
    {
        var availability = configuration.GetOptions<ServicesAvailabilityOptions>();
        if (availability.MongoDb.Enabled)
        {
            services.SetMongoDb(configuration);
            services.SetMongoRepository<TemporaryDocument, Guid>("temporary");
        }

        if (availability.RabbitMq.Enabled)
        {
            services.SetRabbitMqConfiguration(configuration);
        }
        return services;
    }
}