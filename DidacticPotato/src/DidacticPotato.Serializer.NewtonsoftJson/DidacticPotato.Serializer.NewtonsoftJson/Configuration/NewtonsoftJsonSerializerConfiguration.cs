using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DidacticPotato.Serializer.NewtonsoftJson.Configuration;

public static class NewtonsoftJsonSerializerConfiguration
{
    public static IServiceCollection SetNewtonsoftJsonConfiguration(this IServiceCollection services)
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.None,
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
        services.AddSingleton(jsonSerializerSettings);
        services.AddSingleton<ISerializer, NewtonsoftJsonSerializer>();
        return services;
    }
}