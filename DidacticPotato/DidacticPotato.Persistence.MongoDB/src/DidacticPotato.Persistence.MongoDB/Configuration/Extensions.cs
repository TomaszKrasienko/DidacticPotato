using DidacticPotato.Persistence.MongoDB.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DidacticPotato.Persistence.MongoDB;

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
        return services;
    }

    private static IServiceCollection SetOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbOptions>(configuration.GetSection("MongoDb"));
    }
}