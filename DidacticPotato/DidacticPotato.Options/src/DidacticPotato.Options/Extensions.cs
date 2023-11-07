using Microsoft.Extensions.Configuration;

namespace DidacticPotato.Options;

public static class Extensions
{
    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var obj = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(obj);
        return obj;
    }

    public static T GetOptions<T>(this IConfiguration configuration) where T : class, new()
    {
        var obj = new T();
        configuration.Bind(obj);
        return obj;
    }
}