namespace Accelergreat.EntityFramework;

public interface IEntityFrameworkGlobalDataItems
{
    public IReadOnlyCollection<object> GlobalDataItems { get; }
}