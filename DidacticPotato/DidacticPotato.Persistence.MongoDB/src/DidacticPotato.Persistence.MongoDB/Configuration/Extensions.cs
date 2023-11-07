using DidacticPotato.Options;
using DidacticPotato.Persistence.MongoDB.Factories;
using DidacticPotato.Persistence.MongoDB.Factories.Abstractions;
using DidacticPotato.Persistence.MongoDB.Models;
using DidacticPotato.Persistence.MongoDB.Repositories;
using DidacticPotato.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DidacticPotato.Persistence.MongoDB.Configuration;

public static class Extensions
{
    public static IServiceCollection SetMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.SetOptions(configuration);
        services.AddSingleton<IMongoClient>(opt =>
        {
            var options = opt.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddTransient(opt =>
        {
            var client = opt.GetRequiredService<IMongoClient>();
            var options = opt.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return client.GetDatabase(options.Database);
        });

        services.AddTransient<IMongoSessionFactory, MongoSessionFactory>();
        return services;
    }

    private static IServiceCollection SetOptions(this IServiceCollection services, IConfiguration configuration)
        => services.Configure<MongoDbOptions>(configuration.GetSection("MongoDb"));
       


    public static IServiceCollection SetMongoRepository<TEntity, TIdentifier>(this IServiceCollection services,
        string collectionName) where TEntity : IEntity<TIdentifier>
    {
        services.AddTransient<IMongoRepository<TEntity, TIdentifier>>(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return new MongoRepository<TEntity, TIdentifier>(database, collectionName);
        });

        return services;
    }
}