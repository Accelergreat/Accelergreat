# Supporting Microservices (Premium Feature)

This feature is intended for use in solutions where there is more than one API project that need to be tested together.

## Great News!

Not only does Accelergreat support this feature, it also supports debugging across microservices! If you haven't already, please read the [main documentation](index.md).

## GitHub Example

You can find an example of Accelergreat integration tests with a microservice architecture using OpenID Connect (OIDC) and IdentityServer4 on GitHub [here](https://github.com/Accelergreat/Examples/tree/main/Accelegreat.IdpExample).

## Process

1. Reference the API projects in your test project.
2. Alias each API project if using minimal API in .NET 6.0 or above.
3. Create components for each of the APIs derived from KestrelWebAppComponent instead of WebAppComponent.
4. Reference the relative URLs in BuildConfiguration and override URL settings.

### 1. Reference the API projects in your test project.

Add project references for each of the microservice APIs to your Test Assembly.

### 2. Alias each API project if using .NET 6.0.

1. In Solution Explorer, right-click the API project reference (from the Test Assembly) and select "Properties."
2. In the "Aliases" box, provide an alias (e.g. `ExampleApi1`

### 3. Create Components for each of the Apis derived from KestrelWebAppComponent instead of WebAppComponent

``` C#
extern alias ExampleApi1

internal class ExampleApi1Component : KestrelWebAppComponent<Program>
```

### 4. Reference the relative urls in BuildConfiguration and override url settings

``` c#
protected override void BuildConfiguration(IConfigurationBuilder configurationBuilder,
        IReadOnlyAccelergreatPipelineData accelergreatEnvironmentPipelineData)
    {
        configurationBuilder.AddEntityFrameworkDatabaseConnectionString<ExampleContext>("ExampleContext",
            accelergreatEnvironmentPipelineData);
        var mockServerUrl =
            accelergreatEnvironmentPipelineData[
                AccelergreatEnvironmentPipelineDataKeys.KestrelWebAppHttpBaseAddress<MockServer::Program>()].ToString();
        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("MockServer:Url", mockServerUrl!)
        });
        base.BuildConfiguration(configurationBuilder, accelergreatEnvironmentPipelineData);
    }
```