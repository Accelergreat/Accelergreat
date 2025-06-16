namespace Accelergreat.Environments;

internal sealed class AccelergreatEnvironmentPipelineData : Dictionary<string, object>, IAccelergreatEnvironmentPipelineData
{
    internal AccelergreatEnvironmentPipelineData() : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public T Get<T>(string key) => (T)this[key];
}