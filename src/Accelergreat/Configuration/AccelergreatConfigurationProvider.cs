using System.ComponentModel;
using Accelergreat.Exceptions;
using Microsoft.Extensions.Configuration;
using NJsonSchema;

namespace Accelergreat.Configuration;


internal static class AccelergreatConfigurationProvider
{
    internal static async Task<IConfiguration> GetAccelergreatConfigurationAsync()
    {
        var accelergreatEnvironment = Environment.GetEnvironmentVariable("ACCELERGREAT_ENVIRONMENT");

        var configurationBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables("ACCELERGREAT:")
            .AddUserSecrets("accelergreat");

        await AddJsonFileAsync(configurationBuilder, "accelergreat.json");

        if (accelergreatEnvironment is not null)
        {
            await AddJsonFileAsync(configurationBuilder, $"accelergreat.{accelergreatEnvironment}.json");
        }

        return configurationBuilder.Build();
    }

    private static async Task AddJsonFileAsync(IConfigurationBuilder configurationBuilder, string path)
    {
        if (File.Exists(path))
        {
            var json = await File.ReadAllTextAsync(path);

            var schema = await AccelergreatConfigurationSchemaLoader.LoadAsync();

            var validationErrors = schema.Validate(json, SchemaType.JsonSchema);

            if (validationErrors.Count != 0)
            {
                throw new AccelergreatConfigurationValidationsException(schema, path, validationErrors);
            }

            configurationBuilder.AddJsonFile(path);
        }
    }
}