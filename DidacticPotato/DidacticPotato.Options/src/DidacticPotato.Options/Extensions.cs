using Microsoft.Extensions.Configuration;

namespace DidacticPotato.Options;

public static class Extensions
{
    public static T Options<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var obj = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(obj);
        return obj;
    }
}