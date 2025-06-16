namespace Accelergreat.Environments;

public interface IReadOnlyAccelergreatEnvironmentPipelineData : IReadOnlyDictionary<string, object>
{
    T Get<T>(string key);
}