namespace Accelergreat.Environments;

internal sealed class AccelergreatEnvironmentIdAllocator : IAccelergreatEnvironmentIdAllocator
{
    private readonly object _allocationLock;
    private int _value;

    public AccelergreatEnvironmentIdAllocator()
    {
        _allocationLock = new object();
    }

    int IAccelergreatEnvironmentIdAllocator.Allocate()
    {
        int result;

        lock (_allocationLock)
        {
            result = _value;

            _value++;
        }

        return result;
    }
}