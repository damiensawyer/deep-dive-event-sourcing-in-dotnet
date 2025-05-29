using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace BeerSender.Domain.JsonConfiguration;

public class CommandTypeResolver : DefaultJsonTypeInfoResolver
{
    private static readonly List<JsonDerivedType> CommandTypes = new();

    static CommandTypeResolver()
    {
        var commandTypes = typeof(ICommand)
            .Assembly
            .GetTypes()
            .Where(type => 
                type is { IsClass: true, IsAbstract: false }
                && typeof(ICommand).IsAssignableFrom(type));

        foreach (var commandType in commandTypes)
        {
            CommandTypes.Add(new JsonDerivedType(commandType, commandType.Name));
        }
    }
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        Type commandType = typeof(ICommand);
        if (jsonTypeInfo.Type == commandType)
        {
            var polyOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$command-type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType
            };
            foreach (var jsonDerivedType in CommandTypes)
            {
                polyOptions.DerivedTypes.Add(jsonDerivedType);
            }

            jsonTypeInfo.PolymorphismOptions = polyOptions;
        }

        return jsonTypeInfo;
    }
}