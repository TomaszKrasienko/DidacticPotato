using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DidacticPotato.Serializer.NewtonsoftJson;

internal sealed class NewtonsoftJsonSerializer : ISerializer
{
    private JsonSerializerSettings _settings;
    
    public NewtonsoftJsonSerializer(JsonSerializerSettings settings = null)
    {
        _settings = settings ?? new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public string ToJson(object value)
        => JsonConvert.SerializeObject(value, _settings);

    public object ToObject<T>(string json)
        => JsonConvert.DeserializeObject<T>(json, _settings);
}