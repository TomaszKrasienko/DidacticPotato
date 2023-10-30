namespace DidacticPotato.Serializer;

public interface ISerializer
{
    string ToJson(object value);
    object ToObject<T>(string json);
    object ToObject(string json, Type type);
}