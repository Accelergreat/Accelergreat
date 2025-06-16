using System.ComponentModel;
using System.Reflection;
using NJsonSchema;

namespace Accelergreat.Configuration;


internal static class AccelergreatConfigurationSchemaLoader
{
    private static readonly Lazy<Task<JsonSchema>> JsonSchema = new(async () =>
    {
        var assembly = Assembly.GetAssembly(typeof(AccelergreatConfigurationSchemaLoader))!;

        await using var schemaStream = assembly
            .GetManifestResourceStream(AccelergreatConfigurationConstants.SchemaResourceName)!;

        return await NJsonSchema.JsonSchema.FromJsonAsync(schemaStream);
    });
    
    public static Task<JsonSchema> LoadAsync()
    {
        return JsonSchema.Value;
    }
}