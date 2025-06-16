using Accelergreat.Environments;

namespace Accelergreat.Web.Extensions;

public static class AccelergreatEnvironmentPipelineDataWebExtensions
{
    public static HttpClient GetWebAppHttpClient<TEntryPoint>(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TEntryPoint : class
    {
        return accelergreatEnvironmentPipelineData.Get<HttpClient>(
            AccelergreatEnvironmentPipelineDataKeys.WebAppHttpClient<TEntryPoint>());
    }

    internal static void AddWebAppHttpClient<TEntryPoint>(
        this IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        HttpClient httpClient)
        where TEntryPoint : class
    {
        accelergreatEnvironmentPipelineData.Add(
            AccelergreatEnvironmentPipelineDataKeys.WebAppHttpClient<TEntryPoint>(),
            httpClient);
    }

    public static string GetKestrelWebAppHttpBaseAddress<TEntryPoint>(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
        where TEntryPoint : class
    {
        return accelergreatEnvironmentPipelineData.Get<string>(
            AccelergreatEnvironmentPipelineDataKeys.KestrelWebAppHttpBaseAddress<TEntryPoint>());
    }

    internal static void AddKestrelWebAppHttpBaseAddress<TEntryPoint>(
        this IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        string baseAddress)
        where TEntryPoint : class
    {
        accelergreatEnvironmentPipelineData.Add(
            AccelergreatEnvironmentPipelineDataKeys.KestrelWebAppHttpBaseAddress<TEntryPoint>(),
            baseAddress);
    }
}