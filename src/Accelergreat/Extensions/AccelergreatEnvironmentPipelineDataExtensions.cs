using Accelergreat.Environments;

namespace Accelergreat.Extensions;

public static class AccelergreatEnvironmentPipelineDataExtensions
{
    public static int GetEnvironmentId(
        this IReadOnlyAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData)
    {
        return accelergreatEnvironmentPipelineData.Get<int>(
            AccelergreatEnvironmentPipelineDataKeys.EnvironmentId);
    }

    internal static void AddEnvironmentId(
        this IAccelergreatEnvironmentPipelineData accelergreatEnvironmentPipelineData,
        int environmentId)
    {
        accelergreatEnvironmentPipelineData.Add(
            AccelergreatEnvironmentPipelineDataKeys.EnvironmentId,
            environmentId);
    }
}